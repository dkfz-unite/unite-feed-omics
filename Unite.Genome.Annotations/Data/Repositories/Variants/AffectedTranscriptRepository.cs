using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models.Variants;

namespace Unite.Genome.Annotations.Data.Repositories;

internal abstract class AffectedTranscriptRepository<TVariant, TAffectedTranscript>
    where TVariant : Variant
    where TAffectedTranscript : VariantAffectedFeature<TVariant, Transcript>
{
    protected readonly DomainDbContext _dbContext;
    protected readonly VariantRepository<TVariant> _variantRepository;
    protected readonly TranscriptRepository _transcriptRepository;


    public AffectedTranscriptRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _variantRepository = new VariantRepository<TVariant>(dbContext);
        _transcriptRepository = new TranscriptRepository(dbContext);
    }


    public TAffectedTranscript FindOrCreate(AffectedTranscriptModel model)
    {
        return Find(model) ?? Create(model);
    }

    public TAffectedTranscript Find(AffectedTranscriptModel model)
    {
        var variant = _variantRepository.Find(model.Variant);

        if (variant == null)
        {
            return null;
        }

        var transcript = _transcriptRepository.Find(model.Transcript);

        if (transcript == null)
        {
            return null;
        }

        var entity = _dbContext.Set<TAffectedTranscript>()
            .FirstOrDefault(entity =>
                entity.VariantId == variant.Id &&
                entity.FeatureId == transcript.Id
            );

        return entity;
    }

    public TAffectedTranscript Create(
        AffectedTranscriptModel model,
        IEnumerable<TVariant> variantsCache = null,
        IEnumerable<Transcript> transcriptsCache = null)
    {
        var entity = Convert(model, variantsCache, transcriptsCache);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public IEnumerable<TAffectedTranscript> CreateMissing(
        IEnumerable<AffectedTranscriptModel> models,
        IEnumerable<TVariant> variantsCache = null,
        IEnumerable<Transcript> transcriptsCache = null)
    {
        var entitiesToAdd = new List<TAffectedTranscript>();

        foreach (var model in models)
        {
            var entity = Find(model);

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

    protected virtual TAffectedTranscript Convert(
        AffectedTranscriptModel model,
        IEnumerable<TVariant> variantsCache = null,
        IEnumerable<Transcript> transcriptsCache = null)
    {
        var variant = _variantRepository.Find(model.Variant, variantsCache);
        var feature = _transcriptRepository.FindOrCreate(model.Transcript, transcriptsCache);
        var consequences = model.Consequences.Select(type => new Consequence(type)).ToArray();

        var entity = Activator.CreateInstance<TAffectedTranscript>();

        entity.VariantId = variant.Id;
        entity.FeatureId = feature.Id;
        entity.Consequences = consequences;

        return entity;
    }
}
