using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Variants;

namespace Unite.Genome.Feed.Data.Repositories.Variants;

public abstract class VariantOccurrenceRepository<TVariantOccurrenceEntity, TVariantEntity, TVariantModel>
    where TVariantOccurrenceEntity : VariantOccurrence<TVariantEntity>
    where TVariantEntity : Variant
    where TVariantModel : VariantModel
{
    private readonly DomainDbContext _dbContext;
    private readonly VariantRepository<TVariantEntity, TVariantModel> _variantRepository;


    public VariantOccurrenceRepository(
        DomainDbContext dbContext,
        VariantRepository<TVariantEntity, TVariantModel> variantRepository)
    {
        _dbContext = dbContext;
        _variantRepository = variantRepository;
    }


    public TVariantOccurrenceEntity FindOrCreate(
        int analysedSampleId,
        TVariantModel model,
        IEnumerable<TVariantEntity> cache = null)
    {
        return Find(analysedSampleId, model, cache) ?? Create(analysedSampleId, model);
    }

    public TVariantOccurrenceEntity Find(
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

    public TVariantOccurrenceEntity Create(
        int analysedSampleId,
        TVariantModel model,
        IEnumerable<TVariantEntity> cache = null)
    {
        var variant = _variantRepository.FindOrCreate(model, cache);

        return Create(analysedSampleId, variant.Id);
    }

    public IEnumerable<TVariantOccurrenceEntity> CreateMissing(
        int analysedSampleId,
        IEnumerable<TVariantModel> models,
        IEnumerable<TVariantEntity> cache = null)
    {
        var entitiesToAdd = new List<TVariantOccurrenceEntity>();

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

    public IEnumerable<TVariantOccurrenceEntity> CreateAll(
        int analysedSampleId,
        IEnumerable<TVariantModel> models,
        IEnumerable<TVariantEntity> cache = null)
    {
        var entitiesToAdd = new List<TVariantOccurrenceEntity>();

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
        var entitiesToRemove = _dbContext.Set<TVariantOccurrenceEntity>()
            .Where(entity => entity.AnalysedSampleId == analysedSampleId)
            .ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private TVariantOccurrenceEntity Find(int analysedSampleId, long variantId)
    {
        var entity = _dbContext.Set<TVariantOccurrenceEntity>()
            .FirstOrDefault(entity =>
                entity.AnalysedSampleId == analysedSampleId &&
                entity.VariantId == variantId
            );

        return entity;
    }

    private TVariantOccurrenceEntity Create(int analysedSampleId, long variantId)
    {
        var entity = Convert(analysedSampleId, variantId);

        _dbContext.Set<TVariantOccurrenceEntity>().Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private TVariantOccurrenceEntity Convert(int analysedSampleId, long variantId)
    {
        var entity = Activator.CreateInstance<TVariantOccurrenceEntity>();

        entity.AnalysedSampleId = analysedSampleId;
        entity.VariantId = variantId;

        return entity;
    }
}
