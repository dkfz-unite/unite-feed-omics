using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Context.Extensions.Queryable;
using Unite.Data.Context.Repositories.Constants;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome.Analysis.Rna;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Essentials.Extensions;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;


namespace Unite.Genome.Indices.Services;

/// <summary>
/// Cache for indexing variants. This is a stateful service, make sure to clean it up after usage.
/// </summary>
/// <typeparam name="TVariant"></typeparam>
/// <typeparam name="TVariantEntry"></typeparam>
public class VariantIndexingCache<TVariant, TVariantEntry>
    where TVariant : Data.Entities.Genome.Analysis.Dna.Variant
    where TVariantEntry: Data.Entities.Genome.Analysis.Dna.VariantEntry<TVariant>
{
    private static readonly object _lock = new();

    private readonly HashSet<int> _sampleIds = [];
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;

    public IEnumerable<TVariant> Variants { get; private set; }
    public IEnumerable<TVariantEntry> Entries { get; private set; }
    public IEnumerable<GeneExpression> Expressions { get; private set; }
    public IEnumerable<Donor> Donors { get; private set; }
    public IEnumerable<Image> Images { get; private set; }
    public IEnumerable<Specimen> Specimens { get; private set; }
    public IEnumerable<Data.Entities.Genome.Analysis.Sample> Samples { get; private set; }


    public VariantIndexingCache(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }


    public void Load(int[] ids)
    {
        LoadVariants(ids).Wait();
        LoadExpressions().Wait();
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
        Expressions = null;
        Donors = null;
        Images = null;
        Specimens = null;
        Samples = null;
    }


    private async Task LoadVariants(int[] ids)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();

        if (typeof(TVariantEntry) == typeof(SSM.VariantEntry))
            Entries = await GetSsmEntries(ids) as IEnumerable<TVariantEntry>;
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

    private async Task LoadExpressions()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var geneIds = Array.Empty<int>();

        if (typeof(TVariant) == typeof(SSM.Variant))
        {
            geneIds = (Variants as IEnumerable<SSM.Variant>)
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

        Expressions = await dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Where(expression => geneIds.Contains(expression.EntityId))
            .ToArrayAsync();
    }

    private async Task LoadDonors()
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();

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
        await using var dbContext = _dbContextFactory.CreateDbContext();

        var predicate = Predicates.IsImageRelatedSpecimen.Compile();

        var donorIds = Specimens
            .Where(predicate)
            .Select(specimen => specimen.DonorId)
            .Distinct()
            .ToArray();

        Images = await dbContext.Set<Image>()
            .AsNoTracking()
            .IncludeMriImage()
            .IncludeRadiomicsFeatures()
            .Where(image => donorIds.Contains(image.DonorId))
            .ToArrayAsync();
    }

    private async Task LoadSpecimens()
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();

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
        await using var dbContext = _dbContextFactory.CreateDbContext();

        Samples = await dbContext.Set<Data.Entities.Genome.Analysis.Sample>()
            .AsNoTracking()
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => _sampleIds.Contains(sample.Id))
            .ToArrayAsync();
    }


    private async Task<IEnumerable<SSM.VariantEntry>> GetSsmEntries(int[] ids)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return await dbContext.Set<SSM.VariantEntry>()
            .AsNoTracking()
            .Where(entry => ids.Contains(entry.EntityId))
            .ToArrayAsync();
    }

    private async Task<IEnumerable<CNV.VariantEntry>> GetCnvEntries(int[] ids)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        Expression<Func<CNV.VariantEntry, CNV.Variant>> path = entry => entry.Entity;

        return await dbContext.Set<CNV.VariantEntry>()
            .AsNoTracking()
            .Where(path.Join(Predicates.IsInfluentCnv))
            .Where(entry => ids.Contains(entry.EntityId))
            .ToArrayAsync();
    }

    private async Task<IEnumerable<SV.VariantEntry>> GetSvEntries(int[] ids)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return await dbContext.Set<SV.VariantEntry>()
            .AsNoTracking()
            .Where(entry => ids.Contains(entry.EntityId))
            .ToArrayAsync();
    }
}
