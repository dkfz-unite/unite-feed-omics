using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Variants;

namespace Unite.Genome.Feed.Data.Repositories.Variants;

internal abstract class VariantOccurrenceRepository<TVariantOccurrenceEntity, TVariantEntity, TVariantModel>
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


    public TVariantOccurrenceEntity FindOrCreate(int analysedSampleId, TVariantModel model)
    {
        return Find(analysedSampleId, model) ?? Create(analysedSampleId, model);
    }

    public TVariantOccurrenceEntity Find(int analysedSampleId, TVariantModel model)
    {
        var variant = _variantRepository.Find(model);

        if (variant != null)
        {
            return Find(analysedSampleId, variant.Id);
        }

        return null;
    }

    public TVariantOccurrenceEntity Create(int analysedSampleId, TVariantModel model)
    {
        var variant = _variantRepository.FindOrCreate(model);

        return Create(analysedSampleId, variant.Id);
    }

    public IEnumerable<TVariantOccurrenceEntity> CreateOrUpdate(int analysedSampleId, IEnumerable<TVariantModel> models)
    {
        RemoveRedundant(analysedSampleId, models);

        var created = CreateMissing(analysedSampleId, models);

        return created;
    }

    public IEnumerable<TVariantOccurrenceEntity> CreateMissing(int analysedSampleId, IEnumerable<TVariantModel> models)
    {
        var entitiesToAdd = new List<TVariantOccurrenceEntity>();

        foreach (var model in models)
        {
            var variant = _variantRepository.FindOrCreate(model);

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

    public void RemoveRedundant(int analysedSampleId, IEnumerable<TVariantModel> models)
    {
        var entitiesToKeep = new List<long>();

        foreach (var model in models)
        {
            var variant = _variantRepository.Find(model);

            if (variant == null)
            {
                continue;
            }

            var entity = Find(analysedSampleId, variant.Id);

            if (entity != null)
            {
                entitiesToKeep.Add(entity.VariantId);
            }
        }

        var entitiesToRemove = _dbContext.Set<TVariantOccurrenceEntity>()
            .Where(entity => entity.AnalysedSampleId == analysedSampleId && !entitiesToKeep.Contains(entity.VariantId))
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

        _dbContext.Add(entity);
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
