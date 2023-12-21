using Unite.Data.Context;
using Unite.Data.Entities.Genome.Variants;
using Unite.Genome.Feed.Data.Models.Variants;

namespace Unite.Genome.Feed.Data.Repositories.Variants;

public abstract class VariantEntryRepository<TVariantEntryEntity, TVariantEntity, TVariantModel>
    where TVariantEntryEntity : VariantEntry<TVariantEntity>
    where TVariantEntity : Variant
    where TVariantModel : VariantModel
{
    private readonly DomainDbContext _dbContext;
    private readonly VariantRepository<TVariantEntity, TVariantModel> _variantRepository;


    protected VariantEntryRepository(
        DomainDbContext dbContext,
        VariantRepository<TVariantEntity, TVariantModel> variantRepository)
    {
        _dbContext = dbContext;
        _variantRepository = variantRepository;
    }


    public TVariantEntryEntity FindOrCreate(
        int analysedSampleId,
        TVariantModel model,
        IEnumerable<TVariantEntity> cache = null)
    {
        return Find(analysedSampleId, model, cache) ?? Create(analysedSampleId, model);
    }

    public TVariantEntryEntity Find(
        int analysedSampleId,
        TVariantModel model,
        IEnumerable<TVariantEntity> cache = null)
    {
        var variant = _variantRepository.Find(model, cache);

        if (variant != null)
        {
            return Find(analysedSampleId, variant.Id);
        }

        return null;
    }

    public TVariantEntryEntity Create(
        int analysedSampleId,
        TVariantModel model,
        IEnumerable<TVariantEntity> cache = null)
    {
        var variant = _variantRepository.FindOrCreate(model, cache);

        return Create(analysedSampleId, variant.Id);
    }

    public IEnumerable<TVariantEntryEntity> CreateMissing(
        int analysedSampleId,
        IEnumerable<TVariantModel> models,
        IEnumerable<TVariantEntity> cache = null)
    {
        var entitiesToAdd = new List<TVariantEntryEntity>();

        foreach (var model in models)
        {
            var variant = _variantRepository.FindOrCreate(model, cache);

            var entity = Find(analysedSampleId, variant.Id);

            if (entity == null)
            {
                entity = Convert(analysedSampleId, variant.Id);
                entitiesToAdd.Add(entity);
            }
        }

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd;
    }

    public IEnumerable<TVariantEntryEntity> CreateAll(
        int analysedSampleId,
        IEnumerable<TVariantModel> models,
        IEnumerable<TVariantEntity> cache = null)
    {
        var entitiesToAdd = new List<TVariantEntryEntity>();

        foreach (var model in models)
        {
            var variant = _variantRepository.FindOrCreate(model, cache);

            var entity = Convert(analysedSampleId, variant.Id);

            entitiesToAdd.Add(entity);
        }

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd;
    }

    public void RemoveAll(int analysedSampleId)
    {
        var entitiesToRemove = _dbContext.Set<TVariantEntryEntity>()
            .Where(entity => entity.AnalysedSampleId == analysedSampleId)
            .ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private TVariantEntryEntity Find(int analysedSampleId, long variantId)
    {
        var entity = _dbContext.Set<TVariantEntryEntity>()
            .FirstOrDefault(entity =>
                entity.AnalysedSampleId == analysedSampleId &&
                entity.EntityId == variantId
            );

        return entity;
    }

    private TVariantEntryEntity Create(int analysedSampleId, long variantId)
    {
        var entity = Convert(analysedSampleId, variantId);

        _dbContext.Set<TVariantEntryEntity>().Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private static TVariantEntryEntity Convert(int analysedSampleId, long variantId)
    {
        var entity = Activator.CreateInstance<TVariantEntryEntity>();

        entity.AnalysedSampleId = analysedSampleId;
        entity.EntityId = variantId;

        return entity;
    }
}
