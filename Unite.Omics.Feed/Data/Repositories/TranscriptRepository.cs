using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Repositories;

public class TranscriptRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly GeneRepository _geneRepository;


    public TranscriptRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _geneRepository = new GeneRepository(dbContext);
    }


    public Transcript FindOrCreate(TranscriptModel model, IEnumerable<Transcript> cache = null, IEnumerable<Gene> genesCache = null)
    {
        return Find(model, cache) ?? Create(model, genesCache);
    }

    public Transcript Find(TranscriptModel model, IEnumerable<Transcript> cache = null)
    {
        Expression<Func<Transcript, bool>> predicate = (entity) => entity.StableId == model.Id;

        return cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<Transcript>().FirstOrDefault(predicate);
    }

    public IEnumerable<Transcript> Find(IEnumerable<TranscriptModel> models, IEnumerable<Transcript> cache = null)
    {
        var allIdentifiers = models.Select(model => model.Id).ToArray();

        var cachedEntities = cache?.Where(entity => allIdentifiers.Contains(entity.StableId)).ToArray() ?? Array.Empty<Transcript>();

        var cachedIdentifiers = cachedEntities.Select(entity => entity.StableId).ToArray();

        var newIdentifiers = allIdentifiers.Except(cachedIdentifiers).ToArray();

        var newEntities = _dbContext.Set<Transcript>().Where(entity => newIdentifiers.Contains(entity.StableId));

        return Enumerable.Concat(cachedEntities, newEntities).ToArray();
    }

    public Transcript Create(TranscriptModel model, IEnumerable<Gene> genesCache = null)
    {
        var entity = Convert(model, genesCache);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public IEnumerable<Transcript> CreateMissing(IEnumerable<TranscriptModel> models, IEnumerable<Transcript> cache = null, IEnumerable<Gene> genesCache = null)
    {
        var existingIdentifiers = Find(models, cache).Select(entity => entity.StableId).ToArray();

        var entitiesToAdd = new List<Transcript>();

        foreach (var model in models)
        {
            if (existingIdentifiers.Contains(model.Id))
            {
                continue;
            }

            var entity = Find(model);

            if (entity == null)
            {
                entity = Convert(model, genesCache);

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


    private Transcript Convert(TranscriptModel model, IEnumerable<Gene> genesCache = null)
    {
        var entity = new Transcript
        {
            StableId = model.Id,
            Symbol = model.Symbol,
            Description = model.Description,
            Biotype = model.Biotype,
            IsCanonical = model.IsCanonical,
            ChromosomeId = model.Chromosome,
            Start = model.Start,
            End = model.End,
            Strand = model.Strand,
            ExonicLength = model.ExonicLength
        };

        if (model.Gene != null)
        {
            entity.GeneId = _geneRepository.FindOrCreate(model.Gene, genesCache).Id;
        }

        return entity;
    }
}
