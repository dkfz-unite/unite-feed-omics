using Unite.Data.Context;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Genome.Feed.Data.Models.Dna;

namespace Unite.Genome.Feed.Data.Repositories.Dna;

public abstract class AffectedTranscriptRepository<TAffectedTranscriptEntity, TVariantEntity, TVariantModel>
    where TAffectedTranscriptEntity : VariantAffectedFeature<TVariantEntity, Transcript>
    where TVariantEntity : Variant
    where TVariantModel : VariantModel
{
    protected readonly DomainDbContext _dbContext;
    protected readonly VariantRepository<TVariantEntity, TVariantModel> _variantRepository;
    protected readonly TranscriptRepository _transcriptRepository;


    public AffectedTranscriptRepository(DomainDbContext dbContext, VariantRepository<TVariantEntity, TVariantModel> variantRepository)
    {
        _dbContext = dbContext;
        _variantRepository = variantRepository;
        _transcriptRepository = new TranscriptRepository(dbContext);
    }


    public TAffectedTranscriptEntity FindOrCreate(
        AffectedTranscriptModel model,
        IEnumerable<TVariantEntity> variantsCache = null,
        IEnumerable<Transcript> transcriptsCache = null)
    {
        return Find(model, variantsCache, transcriptsCache) ?? Create(model, variantsCache, transcriptsCache);
    }

    public TAffectedTranscriptEntity Find(
        AffectedTranscriptModel model,
        IEnumerable<TVariantEntity> variantsCache = null,
        IEnumerable<Transcript> transcriptsCache = null)
    {
        var variant = _variantRepository.Find(model.VariantId, variantsCache);

        if (variant == null)
        {
            return null;
        }

        var transcript = _transcriptRepository.Find(model.Transcript, transcriptsCache);

        if (transcript == null)
        {
            return null;
        }

        var entity = _dbContext.Set<TAffectedTranscriptEntity>()
            .FirstOrDefault(entity =>
                entity.VariantId == variant.Id &&
                entity.FeatureId == transcript.Id
            );

        return entity;
    }

    public TAffectedTranscriptEntity Create(
        AffectedTranscriptModel model,
        IEnumerable<TVariantEntity> variantsCache = null,
        IEnumerable<Transcript> transcriptsCache = null)
    {
        var entity = Convert(model, variantsCache, transcriptsCache);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public IEnumerable<TAffectedTranscriptEntity> CreateAll(
        IEnumerable<AffectedTranscriptModel> models,
        IEnumerable<TVariantEntity> variantsCache = null,
        IEnumerable<Transcript> transcriptsCache = null)
    {
        var entitiesToAdd = models
            .Select(model => Convert(model, variantsCache, transcriptsCache))
            .ToArray();

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd;
    }

    public void RemoveAll(IEnumerable<int> variantIds)
    {
        var entitiesToRemove = _dbContext.Set<TAffectedTranscriptEntity>()
            .Where(entity => variantIds.Contains(entity.VariantId))
            .ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }

    public IEnumerable<TAffectedTranscriptEntity> CreateMissing(
        IEnumerable<AffectedTranscriptModel> models,
        IEnumerable<TVariantEntity> variantsCache = null,
        IEnumerable<Transcript> transcriptsCache = null)
    {
        var entitiesToAdd = new List<TAffectedTranscriptEntity>();

        foreach (var model in models)
        {
            var entity = Find(model, variantsCache, transcriptsCache);

            if (entity == null)
            {
                entity = Convert(model, variantsCache, transcriptsCache);

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

    protected virtual TAffectedTranscriptEntity Convert(
        AffectedTranscriptModel model,
        IEnumerable<TVariantEntity> variantsCache = null,
        IEnumerable<Transcript> transcriptsCache = null)
    {
        var variant = _variantRepository.Find(model.VariantId, variantsCache);
        var feature = _transcriptRepository.Find(model.Transcript, transcriptsCache);
        var effects = model.Consequences.Select(type => new Effect(type)).ToArray();

        var entity = Activator.CreateInstance<TAffectedTranscriptEntity>();

        entity.VariantId = variant.Id;
        entity.FeatureId = feature.Id;
        entity.Effects = effects;

        return entity;
    }
}
