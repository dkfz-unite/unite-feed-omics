using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Repositories.Constants;
using Unite.Data.Context.Repositories.Extensions.Queryable;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Omics.Analysis.Prot;
using Unite.Data.Entities.Omics.Analysis.Rna;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Essentials.Extensions;

using SM = Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using CNV = Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Omics.Analysis.Dna.Sv;


namespace Unite.Omics.Indices.Services;

public class VariantIndexingCache<TVariant, TVariantEntry> : IndexingCache
    where TVariant : Data.Entities.Omics.Analysis.Dna.Variant
    where TVariantEntry : Data.Entities.Omics.Analysis.Dna.VariantEntry<TVariant>
{
    private static readonly object _lock = new();
    
    protected readonly VariantsRepository _variantsRepository;

    private readonly HashSet<int> _sampleIds = [];

    /// <summary>
    /// Cache for indexing variants. This is a stateful service, make sure to clean it up after usage.
    /// </summary>
    /// <typeparam name="TVariant"></typeparam>
    /// <typeparam name="TVariantEntry"></typeparam>
    public VariantIndexingCache(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _variantsRepository = new VariantsRepository(dbContextFactory);
    }

    public IEnumerable<TVariant> Variants { get; private set; }
    public IEnumerable<TVariantEntry> Entries { get; private set; }
    public IEnumerable<GeneExpression> GeneExpressions { get; private set; }
    public IEnumerable<ProteinExpression> ProteinExpressions { get; private set; }
    public IEnumerable<Donor> Donors { get; private set; }
    public IEnumerable<Image> Images { get; private set; }
    public IEnumerable<Specimen> Specimens { get; private set; }
    public IEnumerable<Data.Entities.Omics.Analysis.Sample> Samples { get; private set; }
    public Dictionary<int, TVariant[]> SimilarVariants { get; set; }


    protected override void Load(int[] ids)
    {
        LoadVariants(ids).Wait();
        LoadSimilarVariants(ids).Wait();
        LoadGeneExpressions().Wait();
        LoadProteinExpressions().Wait();
        LoadSamples().Wait();
        LoadSpecimens().Wait();
        LoadImages().Wait();
        LoadDonors().Wait();
    }

    public void Clear()
    {
        _sampleIds.Clear();

        Variants = null;
        Entries = null;
        GeneExpressions = null;
        Donors = null;
        Images = null;
        Specimens = null;
        Samples = null;
    }

    private async Task LoadSimilarVariants(int[] ids)
    {
        await using var dbContext = DbContextFactory.CreateDbContext();

        foreach (var id in ids)
        {
            var variantIds = _variantsRepository.GetSimilarVariants<TVariant>([id]).Result;
            
            var similarVariantIds = dbContext.Set<TVariant>()
                .AsNoTracking()
                .Where(entity => variantIds.Contains(entity.Id))
                .ToArray();

            SimilarVariants[id] = similarVariantIds;
        }
    }

    private async Task LoadVariants(int[] ids)
    {
        await using var dbContext = DbContextFactory.CreateDbContext();

        if (typeof(TVariantEntry) == typeof(SM.VariantEntry))
            Entries = await GetSmEntries(ids) as IEnumerable<TVariantEntry>;
        else if (typeof(TVariantEntry) == typeof(CNV.VariantEntry))
            Entries = await GetCnvEntries(ids) as IEnumerable<TVariantEntry>;
        else if (typeof(TVariantEntry) == typeof(SV.VariantEntry))
            Entries = await GetSvEntries(ids) as IEnumerable<TVariantEntry>;

        var variantIds = Entries
            .Select(entry => entry.EntityId)
            .Distinct()
            .ToArray();

        var sampleIds = Entries
            .Select(entry => entry.SampleId)
            .Distinct()
            .ToArray();

        Variants = await dbContext.Set<TVariant>()
            .AsNoTracking()
            .IncludeAffectedTranscripts()
            .Where(variant => variantIds.Contains(variant.Id))
            .ToArrayAsync();

        _sampleIds.AddRange(sampleIds);
    }

    private async Task LoadGeneExpressions()
    {
        using var dbContext = DbContextFactory.CreateDbContext();

        var geneIds = Array.Empty<int>();

        if (typeof(TVariant) == typeof(SM.Variant))
        {
            geneIds = (Variants as IEnumerable<SM.Variant>)
                .SelectMany(variant => variant.AffectedTranscripts)
                .Where(transcript => transcript.Feature.GeneId != null)
                .Select(transcript => transcript.Feature.GeneId.Value)
                .Distinct()
                .ToArray();
        }
        else if (typeof(TVariant) == typeof(CNV.Variant))
        {
            geneIds = (Variants as IEnumerable<CNV.Variant>)
                .SelectMany(variant => variant.AffectedTranscripts)
                .Where(transcript => transcript.Feature.GeneId != null)
                .Select(transcript => transcript.Feature.GeneId.Value)
                .Distinct()
                .ToArray();
        }
        else if (typeof(TVariant) == typeof(SV.Variant))
        {
            geneIds = (Variants as IEnumerable<SV.Variant>)
                .SelectMany(variant => variant.AffectedTranscripts)
                .Where(transcript => transcript.Feature.GeneId != null)
                .Select(transcript => transcript.Feature.GeneId.Value)
                .Distinct()
                .ToArray();
        }

        GeneExpressions = await dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Where(expression => geneIds.Contains(expression.EntityId))
            .ToArrayAsync();
    }

    private async Task LoadProteinExpressions()
    {
        using var dbContext = DbContextFactory.CreateDbContext();

        var proteinIds = Array.Empty<int>();

        if (typeof(TVariant) == typeof(SM.Variant))
        {
            proteinIds = (Variants as IEnumerable<SM.Variant>)
                .SelectMany(variant => variant.AffectedTranscripts)
                .Where(transcript => transcript.Feature.Protein != null)
                .Select(transcript => transcript.Feature.Protein.Id)
                .Distinct()
                .ToArray();
        }
        else if (typeof(TVariant) == typeof(CNV.Variant))
        {
            proteinIds = (Variants as IEnumerable<CNV.Variant>)
                .SelectMany(variant => variant.AffectedTranscripts)
                .Where(transcript => transcript.Feature.Protein != null)
                .Select(transcript => transcript.Feature.Protein.Id)
                .Distinct()
                .ToArray();
        }
        else if (typeof(TVariant) == typeof(SV.Variant))
        {
            proteinIds = (Variants as IEnumerable<SV.Variant>)
                .SelectMany(variant => variant.AffectedTranscripts)
                .Where(transcript => transcript.Feature.Protein != null)
                .Select(transcript => transcript.Feature.Protein.Id)
                .Distinct()
                .ToArray();
        }

        ProteinExpressions = await dbContext.Set<ProteinExpression>()
            .AsNoTracking()
            .Where(expression => proteinIds.Contains(expression.EntityId))
            .ToArrayAsync();
    }

    private async Task LoadDonors()
    {
        await using var dbContext = DbContextFactory.CreateDbContext();

        var donorIds = Specimens
            .Select(specimen => specimen.DonorId)
            .Distinct()
            .ToArray();

        Donors = await dbContext.Set<Donor>()
            .AsNoTracking()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeProjects()
            .IncludeStudies()
            .Where(donor => donorIds.Contains(donor.Id))
            .ToArrayAsync();
    }

    private async Task LoadImages()
    {
        await using var dbContext = DbContextFactory.CreateDbContext();

        var predicate = Predicates.IsImageRelatedSpecimen.Compile();

        var donorIds = Specimens
            .Where(predicate)
            .Select(specimen => specimen.DonorId)
            .Distinct()
            .ToArray();

        Images = await dbContext.Set<Image>()
            .AsNoTracking()
            .IncludeMrImage()
            .IncludeRadiomicsFeatures()
            .Where(image => donorIds.Contains(image.DonorId))
            .ToArrayAsync();
    }

    private async Task LoadSpecimens()
    {
        await using var dbContext = DbContextFactory.CreateDbContext();

        var specimenIds = Samples
            .Select(sample => sample.SpecimenId)
            .Distinct()
            .ToArray();

        Specimens = await dbContext.Set<Specimen>()
            .AsNoTracking()
            .IncludeMaterial()
            .IncludeLine()
            .IncludeOrganoid()
            .IncludeXenograft()
            .IncludeMolecularData()
            .IncludeInterventions()
            .IncludeDrugScreenings()
            .Where(specimen => specimenIds.Contains(specimen.Id))
            .ToArrayAsync();
    }

    private async Task LoadSamples()
    {
        await using var dbContext = DbContextFactory.CreateDbContext();

        Samples = await dbContext.Set<Data.Entities.Omics.Analysis.Sample>()
            .AsNoTracking()
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => _sampleIds.Contains(sample.Id))
            .ToArrayAsync();
    }


    private async Task<IEnumerable<SM.VariantEntry>> GetSmEntries(int[] ids)
    {
        using var dbContext = DbContextFactory.CreateDbContext();

        return await dbContext.Set<SM.VariantEntry>()
            .AsNoTracking()
            .Where(entry => ids.Contains(entry.EntityId))
            .ToArrayAsync();
    }

    private async Task<IEnumerable<CNV.VariantEntry>> GetCnvEntries(int[] ids)
    {
        using var dbContext = DbContextFactory.CreateDbContext();

        Expression<Func<CNV.VariantEntry, CNV.Variant>> path = entry => entry.Entity;

        return await dbContext.Set<CNV.VariantEntry>()
            .AsNoTracking()
            .Where(path.Join(Predicates.IsInfluentCnv))
            .Where(entry => ids.Contains(entry.EntityId))
            .ToArrayAsync();
    }

    private async Task<IEnumerable<SV.VariantEntry>> GetSvEntries(int[] ids)
    {
        using var dbContext = DbContextFactory.CreateDbContext();

        return await dbContext.Set<SV.VariantEntry>()
            .AsNoTracking()
            .Where(entry => ids.Contains(entry.EntityId))
            .ToArrayAsync();
    }
}
