using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories.Extensions.Queryable;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Data.Entities.Specimens;

namespace Unite.Omics.Indices.Services;

public class CnvProfileIndexingCache : IndexingCache
{
    public CnvProfileIndexingCache(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    public IEnumerable<Profile> CnvProfiles { get; private set; }
    public IEnumerable<Specimen> Specimens { get; private set; }
    public IEnumerable<Data.Entities.Omics.Analysis.Sample> Samples { get; private set; }

    protected override void Load(int[] ids)
    {
        LoadCnvProfiles(ids).Wait();
        LoadSamples().Wait();
        LoadSpecimens().Wait();
    }

    async Task LoadCnvProfiles(int[] ids)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        CnvProfiles = await dbContext.CnvProfiles
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToArrayAsync();
    }

    private async Task LoadSamples()
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

        var sampleIds = CnvProfiles.Select(p => p.SampleId).Distinct().ToArray();

        Samples = await dbContext.OmicsSamples
            .AsNoTracking()
            .Where(s => sampleIds.Contains(s.Id))
            .ToListAsync();
    }
    
    private async Task LoadSpecimens()
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();

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
}