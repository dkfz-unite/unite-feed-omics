using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna;
using Unite.Omics.Feed.Data.Models.Dna;

namespace Unite.Omics.Feed.Data.Repositories.Dna;

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


    public TVariantEntryEntity FindOrCreate(int sampleId, TVariantModel model, IEnumerable<TVariantEntity> cache = null)
    {
        return Find(sampleId, model, cache) ?? Create(sampleId, model);
    }

    public TVariantEntryEntity Find(int sampleId, TVariantModel model, IEnumerable<TVariantEntity> cache = null)
    {
        var variant = _variantRepository.Find(model, cache);

        if (variant != null)
        {
            return Find(sampleId, variant.Id);
        }

        return null;
    }

    public TVariantEntryEntity Create(int sampleId, TVariantModel model, IEnumerable<TVariantEntity> cache = null)
    {
        var variant = _variantRepository.FindOrCreate(model, cache);

        return Create(sampleId, variant.Id);
    }

    public IEnumerable<TVariantEntryEntity> CreateMissing(int sampleId, IEnumerable<TVariantModel> models, IEnumerable<TVariantEntity> cache = null)
    {
        var entitiesToAdd = new List<TVariantEntryEntity>();

        foreach (var model in models)
        {
            var variant = _variantRepository.FindOrCreate(model, cache);

            var entity = Find(sampleId, variant.Id);

            if (entity == null)
            {
                entity = Convert(sampleId, variant.Id);
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

    public IEnumerable<TVariantEntryEntity> CreateAll(int sampleId, IEnumerable<TVariantModel> models, IEnumerable<TVariantEntity> cache = null)
    {
        var entitiesToAdd = new List<TVariantEntryEntity>();

        foreach (var model in models)
        {
            var variant = _variantRepository.FindOrCreate(model, cache);

            var entity = Convert(sampleId, variant.Id);

            entitiesToAdd.Add(entity);
        }

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd;
    }

    public void RemoveAll(int sampleId)
    {
        var entitiesToRemove = _dbContext.Set<TVariantEntryEntity>()
            .Where(entity => entity.SampleId == sampleId)
            .ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private TVariantEntryEntity Find(int sampleId, int variantId)
    {
        var entity = _dbContext.Set<TVariantEntryEntity>()
            .FirstOrDefault(entity =>
                entity.SampleId == sampleId &&
                entity.EntityId == variantId
            );

        return entity;
    }

    private TVariantEntryEntity Create(int sampleId, int variantId)
    {
        var entity = Convert(sampleId, variantId);

        _dbContext.Set<TVariantEntryEntity>().Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private static TVariantEntryEntity Convert(int sampleId, int variantId)
    {
        var entity = Activator.CreateInstance<TVariantEntryEntity>();

        entity.SampleId = sampleId;
        entity.EntityId = variantId;

        return entity;
    }
}
