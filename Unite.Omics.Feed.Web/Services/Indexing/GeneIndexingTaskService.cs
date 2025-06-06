using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;

namespace Unite.Omics.Feed.Web.Services.Indexing;

public class GeneIndexingTaskService : IndexingTaskService<Gene, int>
{
    protected override int BucketSize => 1000;
    private readonly GenesRepository _genesRepository;


    public GeneIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _genesRepository = new GenesRepository(dbContextFactory);
    }


    public override void CreateTasks()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        IterateEntities<Gene, int>(gene => true, gene => gene.Id, genes =>
        {
            CreateTasks(IndexingTaskType.Gene, genes);
        });

        transaction.Commit();
    }

    public override void CreateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var genes = dbContext.Set<Gene>()
                .Where(gene => chunk.Contains(gene.Id))
                .Select(gene => gene.Id)
                .ToArray();

            CreateTasks(IndexingTaskType.Gene, genes);
        });

        transaction.Commit();
    }

    public override void PopulateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var genes = dbContext.Set<Gene>()
                .Where(gene => chunk.Contains(gene.Id))
                .Select(gene => gene.Id)
                .ToArray();

            CreateProjectIndexingTasks(genes);
            CreateDonorIndexingTasks(genes);
            CreateImageIndexingTasks(genes);
            CreateSpecimenIndexingTasks(genes);
            CreateGeneIndexingTasks(genes);
        });

        transaction.Commit();
    }


    protected override IEnumerable<int> LoadRelatedProjects(IEnumerable<int> keys)
    {
        var defaultProjects = LoadDefaultProjects();
        var relatedProjects = _genesRepository.GetRelatedProjects(keys).Result;
        
        return Enumerable.Concat(defaultProjects, relatedProjects);
    }

    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
    {
        return _genesRepository.GetRelatedDonors(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
    {
        return _genesRepository.GetRelatedImages(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        return _genesRepository.GetRelatedSpecimens(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        return keys;
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
}
