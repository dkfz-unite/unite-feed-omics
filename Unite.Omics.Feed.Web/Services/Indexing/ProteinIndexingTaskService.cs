using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;

namespace Unite.Omics.Feed.Web.Services.Indexing;

public class ProteinIndexingTaskService : IndexingTaskService<Protein, int>
{
    protected override int BucketSize => 1000;
    protected readonly ProteinsRepository _proteinsRepository;


    public ProteinIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _proteinsRepository = new ProteinsRepository(dbContextFactory);
    }


    public override void CreateTasks()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        IterateEntities<Protein, int>(protein => true, protein => protein.Id, proteins =>
        {
            CreateTasks(IndexingTaskType.Protein, proteins);
        });

        transaction.Commit();
    }

    public override void CreateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var genes = dbContext.Set<Protein>()
                .Where(protein => chunk.Contains(protein.Id))
                .Select(protein => protein.Id)
                .ToArray();

            CreateTasks(IndexingTaskType.Protein, genes);
        });

        transaction.Commit();
    }

    public override void PopulateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var proteins = dbContext.Set<Protein>()
                .Where(protein => chunk.Contains(protein.Id))
                .Select(protein => protein.Id)
                .ToArray();

            // TODO: Revise related entities to be indexed when proteins are updated.
            CreateProjectIndexingTasks(proteins);
            CreateDonorIndexingTasks(proteins);
            CreateImageIndexingTasks(proteins);
            CreateSpecimenIndexingTasks(proteins);
            CreateGeneIndexingTasks(proteins);
            CreateProteinIndexingTasks(proteins);
        });

        transaction.Commit();
    }


    protected override IEnumerable<int> LoadRelatedProjects(IEnumerable<int> keys)
    {
        var defaultProjects = LoadDefaultProjects();
        var relatedProjects = _proteinsRepository.GetRelatedProjects(keys).Result;
        
        return Enumerable.Concat(defaultProjects, relatedProjects);
    }

    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
    {
        return _proteinsRepository.GetRelatedDonors(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
    {
        return _proteinsRepository.GetRelatedImages(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        return _proteinsRepository.GetRelatedSpecimens(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedProteins(IEnumerable<int> keys)
    {
        return keys;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        return _proteinsRepository.GetRelatedGenes(keys).Result;
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
