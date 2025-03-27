using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Repositories;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Essentials.Extensions;

using SSM = Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using CNV = Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using SV = Unite.Data.Entities.Genome.Analysis.Dna.Sv;


namespace Unite.Genome.Feed.Web.Services.Indexing;

public abstract class VariantIndexingTaskService<TV> : IndexingTaskService<Variant, int>
    where TV : Variant
{
    protected override int BucketSize => 1000;
    protected readonly VariantsRepository _variantsRepository;


    public VariantIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        _variantsRepository = new VariantsRepository(dbContextFactory);
    }


    public override void CreateTasks()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        IterateEntities<TV, int>(variant => true, variant => variant.Id, variants =>
        {
            CreateVariantIndexingTasks(variants);
        });

        transaction.Commit();
    }

    public override void CreateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();

        keys.Iterate(BucketSize, (chunk) =>
        {
            var variants = dbContext.Set<TV>()
                .Where(variant => chunk.Contains(variant.Id))
                .Select(variant => variant.Id)
                .ToArray();

            CreateVariantIndexingTasks(variants);
        });

        transaction.Commit();
    }

    public override void PopulateTasks(IEnumerable<int> keys)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var transaction = dbContext.Database.BeginTransaction();
        var similarKeys = Enumerable.Empty<int>();
        var allKeys = Enumerable.Empty<int>();
        
        if (typeof(TV) == typeof(CNV.Variant))
            similarKeys = _variantsRepository.GetSimilarVariants<CNV.Variant>(keys).Result;
        else if (typeof(TV) == typeof(SV.Variant))
            similarKeys = _variantsRepository.GetSimilarVariants<SV.Variant>(keys).Result;

        allKeys = Enumerable.Concat(keys, similarKeys);

        allKeys.Iterate(BucketSize, (chunk) =>
        {
            var variants = dbContext.Set<TV>()
                .Where(variant => chunk.Contains(variant.Id))
                .Select(variant => variant.Id)
                .ToArray();

            CreateProjectIndexingTasks(variants);
            CreateDonorIndexingTasks(variants);
            CreateImageIndexingTasks(keys);
            CreateSpecimenIndexingTasks(variants);
            CreateGeneIndexingTasks(variants);
            CreateVariantIndexingTasks(variants);
        });

        transaction.Commit();
    }


    protected override IEnumerable<int> LoadRelatedProjects(IEnumerable<int> keys)
    {
        var defaultProjects = LoadDefaultProjects();
        var relatedProjects = _variantsRepository.GetRelatedProjects<TV>(keys).Result;

        return Enumerable.Concat(defaultProjects, relatedProjects);
    }

    protected override IEnumerable<int> LoadRelatedDonors(IEnumerable<int> keys)
    {
        return _variantsRepository.GetRelatedDonors<TV>(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedImages(IEnumerable<int> keys)
    {
        return _variantsRepository.GetRelatedImages<TV>(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSpecimens(IEnumerable<int> keys)
    {
        return _variantsRepository.GetRelatedSpecimens<TV>(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedGenes(IEnumerable<int> keys)
    {
        return _variantsRepository.GetRelatedGenes<TV>(keys).Result;
    }

    protected override IEnumerable<int> LoadRelatedSsms(IEnumerable<int> keys)
    {
        if (typeof(TV) == typeof(SSM.Variant))
            return keys;
        
        return [];
    }

    protected override IEnumerable<int> LoadRelatedCnvs(IEnumerable<int> keys)
    {
        if (typeof(TV) == typeof(CNV.Variant))
            return keys;
        
        return [];
    }

    protected override IEnumerable<int> LoadRelatedSvs(IEnumerable<int> keys)
    {
        if (typeof(TV) == typeof(SV.Variant))
            return keys;
        
        return [];
    }
}
