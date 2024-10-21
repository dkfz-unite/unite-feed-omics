using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Extensions.Queryable;
using Unite.Data.Context.Repositories.Constants;
using Unite.Data.Entities.Donors;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Data.Entities.Genome.Analysis.Rna;
using Unite.Data.Entities.Images;
using Unite.Data.Entities.Specimens;
using Unite.Essentials.Extensions;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Genome.Indices.Services;

public class GenesIndexingCache
{
    private static readonly object _lock = new();

    private readonly HashSet<int> _sampleIds = [];
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    
    public IEnumerable<SSM.AffectedTranscript> SsmTranscripts { get; private set; }
    public IEnumerable<CNV.AffectedTranscript> CnvTranscripts { get; private set; }
    public IEnumerable<SV.AffectedTranscript> SvTranscripts { get; private set; }
    public IEnumerable<Gene> Genes { get; private set; }
    public IEnumerable<SSM.Variant> Ssms { get; private set; }
    public IEnumerable<CNV.Variant> Cnvs { get; private set; }
    public IEnumerable<SV.Variant> Svs { get; private set; }
    public IEnumerable<GeneExpression> ExpEntries { get; private set; }
    public IEnumerable<SSM.VariantEntry> SsmEntries { get; private set; }
    public IEnumerable<CNV.VariantEntry> CnvEntries { get; private set; }
    public IEnumerable<SV.VariantEntry> SvEntries { get; private set; }
    public IEnumerable<Donor> Donors { get; private set; }
    public IEnumerable<Image> Images { get; private set; }
    public IEnumerable<Specimen> Specimens { get; private set; }
    public IEnumerable<Sample> Samples { get; private set; }
    
    
    public GenesIndexingCache(IDbContextFactory<DomainDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }


    public void Load(int[] ids)
    {
        Task.WaitAll
        ([
            LoadExpressions(ids),
            LoadSsmTranscripts(ids),
            LoadCnvTranscripts(ids),
            LoadSvTranscripts(ids)
        ]);

        Task.WaitAll
        ([
            LoadGenes(ids),
            LoadSsms(),
            LoadCnvs(),
            LoadSvs()
        ]);

        LoadSamples().Wait();
        LoadSpecimens().Wait();
        LoadImages().Wait();
        LoadDonors().Wait();
    }

    public void Clear()
    {
        _sampleIds.Clear();

        SsmTranscripts = null;
        CnvTranscripts = null;
        SvTranscripts = null;
        Genes = null;
        Ssms = null;
        Cnvs = null;
        Svs = null;
        ExpEntries = null;
        SsmEntries = null;
        CnvEntries = null;
        SvEntries = null;
        Donors = null;
        Images = null;
        Specimens = null;
        Samples = null;
    }


    private async Task LoadExpressions(int[] ids)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        ExpEntries = await dbContext.Set<GeneExpression>()
            .AsNoTracking()
            .Where(expression => ids.Contains(expression.EntityId))
            .ToArrayAsync();

        // var sampleIds = ExpEntries.Select(expression => expression.SampleId).Distinct().ToArray();

        // lock (_lock)
        // {
        //     _sampleIds.AddRange(sampleIds);
        // }
    }

    private async Task LoadSsmTranscripts(int[] ids)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        SsmTranscripts = await dbContext.Set<SSM.AffectedTranscript>()
            .AsNoTracking()
            .Include(affectedTranscript => affectedTranscript.Feature.Protein)
            .Where(affectedTranscript => affectedTranscript.Feature.Protein != null)
            .Where(affectedTranscript => ids.Contains(affectedTranscript.Feature.GeneId.Value))
            .ToArrayAsync();
    }

    private async Task LoadCnvTranscripts(int[] ids)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        CnvTranscripts = await dbContext.Set<CNV.AffectedTranscript>()
            .AsNoTracking()
            .Include(affectedTranscript => affectedTranscript.Feature.Protein)
            .Where(affectedTranscript => affectedTranscript.Feature.Protein != null)
            .Where(affectedTranscript => ids.Contains(affectedTranscript.Feature.GeneId.Value))
            .ToArrayAsync();
    }

    private async Task LoadSvTranscripts(int[] ids)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        SvTranscripts = await dbContext.Set<SV.AffectedTranscript>()
            .AsNoTracking()
            .Include(affectedTranscript => affectedTranscript.Feature.Protein)
            .Where(affectedTranscript => affectedTranscript.Feature.Protein != null)
            .Where(affectedTranscript => ids.Contains(affectedTranscript.Feature.GeneId.Value))
            .ToArrayAsync();
    }

    private async Task LoadGenes(int[] ids)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        Genes = await dbContext.Set<Gene>()
            .AsNoTracking()
            .Where(gene => ids.Contains(gene.Id))
            .ToArrayAsync();
    }

    private async Task LoadSsms()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var variantIds = SsmTranscripts.Select(affectedTranscript => affectedTranscript.VariantId).Distinct().ToArray();

        SsmEntries = await dbContext.Set<SSM.VariantEntry>()
            .AsNoTracking()
            .Include(entry => entry.Entity)
            .Where(entry => variantIds.Contains(entry.EntityId))
            .ToArrayAsync();

        Ssms = SsmEntries
            .Select(entry => entry.Entity)
            .DistinctBy(variant => variant.Id)
            .ToArray();

        SsmEntries.ForEach(entry => entry.Entity = null);

        lock (_lock)
        {
            _sampleIds.AddRange(SsmEntries.Select(entry => entry.SampleId));
        }
    }

    private async Task LoadCnvs()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var predicate = Predicates.IsInfluentCnv.Compile();

        var variantIds = CnvTranscripts.Select(affectedTranscript => affectedTranscript.VariantId).Distinct().ToArray();

        CnvEntries = await dbContext.Set<CNV.VariantEntry>()
            .AsNoTracking()
            .Include(entry => entry.Entity)
            .Where(entry => variantIds.Contains(entry.EntityId))
            .ToArrayAsync();

        Cnvs = CnvEntries
            .Select(entry => entry.Entity)
            .Where(predicate)
            .DistinctBy(variant => variant.Id)
            .ToArray();

        CnvEntries.ForEach(entry => entry.Entity = null);

        lock (_lock)
        {
            _sampleIds.AddRange(CnvEntries.Select(entry => entry.SampleId));
        }
    }

    private async Task LoadSvs()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var variantIds = SvTranscripts.Select(affectedTranscript => affectedTranscript.VariantId).Distinct().ToArray();

        SvEntries = await dbContext.Set<SV.VariantEntry>()
            .AsNoTracking()
            .Include(entry => entry.Entity)
            .Where(entry => variantIds.Contains(entry.EntityId))
            .ToArrayAsync();

        Svs = SvEntries
            .Select(entry => entry.Entity)
            .DistinctBy(variant => variant.Id)
            .ToArray();

        SvEntries.ForEach(entry => entry.Entity = null);

        lock (_lock)
        {
            _sampleIds.AddRange(SvEntries.Select(entry => entry.SampleId));
        }
    }

    private async Task LoadDonors()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var ids = Specimens
            .Select(specimen => specimen.DonorId)
            .Distinct()
            .ToArray();

        Donors = await dbContext.Set<Donor>()
            .AsNoTracking()
            .IncludeClinicalData()
            .IncludeTreatments()
            .IncludeProjects()
            .IncludeStudies()
            .ToArrayAsync();
    }

    private async Task LoadImages()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var predicate = Predicates.IsImageRelatedSpecimen.Compile();

        var ids = Specimens
            .Where(predicate)
            .Select(specimen => specimen.DonorId)
            .Distinct()
            .ToArray();

        Images = await dbContext.Set<Image>()
            .AsNoTracking()
            .IncludeMriImage()
            .IncludeRadiomicsFeatures()
            .Where(image => ids.Contains(image.DonorId))
            .ToArrayAsync();
    }

    private async Task LoadSpecimens()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var ids = Samples
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
            .Where(specimen => ids.Contains(specimen.Id))
            .ToArrayAsync();
    }

    private async Task LoadSamples()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        Samples = await dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Analysis)
            .Include(sample => sample.Resources)
            .Where(sample => _sampleIds.Contains(sample.Id))
            .ToArrayAsync();
    }
}
