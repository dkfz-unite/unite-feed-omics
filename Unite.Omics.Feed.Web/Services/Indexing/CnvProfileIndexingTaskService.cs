using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;

namespace Unite.Omics.Feed.Web.Services.Indexing;

public class CnvProfileIndexingTaskService : IndexingTaskService<Profile, int>
{
    private readonly CnvProfilesRepository _profilesRepository;

    public CnvProfileIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _profilesRepository = new CnvProfilesRepository(dbContextFactory);
    }

    //TODO: are those CreateTasks methods needed?
    //TODO: the implementation is very similar along the task services
    public override void CreateTasks()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        IterateEntities<Profile, int>(profile => true, profile => profile.Id, profile =>
        {
            CreateTasks(IndexingTaskType.CNVProfile, profile);
        });

        transaction.Commit();
    }

    public override void CreateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var profiles = dbContext.CnvProfiles
                .Where(profile => chunk.Contains(profile.Id))
                .Select(profile => profile.Id)
                .ToArray();

            CreateTasks(IndexingTaskType.CNVProfile, profiles);
        });

        transaction.Commit();
    }

    public override void PopulateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            //TODO: do not make any direct DB queries from this layer
            var profiles = dbContext.CnvProfiles
                .Where(profile => chunk.Contains(profile.Id))
                .Select(profile => profile.Id)
                .ToArray();

            CreateProjectIndexingTasks(profiles);
            CreateDonorIndexingTasks(profiles);
            CreateImageIndexingTasks(profiles);
            CreateSpecimenIndexingTasks(profiles);
            CreateCnvProfileIndexingTasks(profiles);
        });

        transaction.Commit();
    }

    protected override IEnumerable<int> LoadRelatedProjects(IEnumerable<int> keys)
    {
        return _profilesRepository.GetRelatedProjects(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
    {
        return _profilesRepository.GetRelatedDonors(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
    {
        return _profilesRepository.GetRelatedImages(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        return _profilesRepository.GetRelatedSpecimens(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedCnvProfiles(IEnumerable<int> keys)
    {
        return keys;
    }

    protected override IEnumerable<int> LoadRelatedProteins(IEnumerable<int> keys)
    {
        return [];
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        return [];
    }

    protected override IEnumerable<int> LoadRelatedSms(IEnumerable<int> keys)
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

    protected override int BucketSize => 1000;
}