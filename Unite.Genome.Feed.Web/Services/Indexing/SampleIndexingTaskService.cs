using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Genome.Analysis;

namespace Unite.Genome.Feed.Web.Services.Indexing;

public class SampleIndexingTaskService : IndexingTaskService<Sample, int>
{
    protected override int BucketSize => 1000;
    private readonly SpecimensRepository _specimensRepository;


    public SampleIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _specimensRepository = new SpecimensRepository(dbContextFactory);
    }

    
    public override void CreateTasks()
    {
        throw new NotImplementedException();
    }

    public override void CreateTasks(IEnumerable<int> keys)
    {
        throw new NotImplementedException();
    }

    public override void PopulateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        CreateProjectIndexingTasks(keys);
        CreateDonorIndexingTasks(keys);
        CreateImageIndexingTasks(keys);
        CreateSpecimenIndexingTasks(keys);

        transaction.Commit();
    }

    protected override IEnumerable<int> LoadRelatedProjects(IEnumerable<int> keys)
    {
        return [];
    }

    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Sample>()
            .AsNoTracking()
            .Include(sample => sample.Specimen)
            .Where(sample => keys.Contains(sample.Id))
            .Select(sample => sample.Specimen.DonorId)
            .Distinct()
            .ToArray();
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
    {
        var specimens = LoadRelatedSpecimens(keys);

        return _specimensRepository.GetRelatedImages(specimens).Result;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Set<Sample>()
            .AsNoTracking()
            .Where(sample => keys.Contains(sample.Id))
            .Select(sample => sample.SpecimenId)
            .Distinct()
            .ToArray();
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        return [];
    }

    protected override IEnumerable<int> LoadRelatedSsms(IEnumerable<int> keys)
    {
        return [];
    }

    protected override IEnumerable<int> LoadRelatedCnvs(IEnumerable<int> keys)
    {
        return [];
    }

    protected override IEnumerable<int> LoadRelatedSvs(IEnumerable<int> keys)
    {
        return [];
    }
}
