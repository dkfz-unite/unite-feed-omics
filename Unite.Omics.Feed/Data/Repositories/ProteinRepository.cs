using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Repositories;

public class ProteinRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly TranscriptRepository _transcriptRepository;


    public ProteinRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _transcriptRepository = new TranscriptRepository(dbContext);
    }


    public Protein FindOrCreate(ProteinModel model, IEnumerable<Protein> cache = null, IEnumerable<Transcript> transcriptsCache = null, IEnumerable<Gene> genesCache = null)
    {
        return Find(model, cache) ?? Create(model, transcriptsCache, genesCache);
    }

    public Protein Find(ProteinModel model, IEnumerable<Protein> cache = null)
    {
        Expression<Func<Protein, bool>> predicate = (entity) => entity.StableId == model.Id;

        return cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<Protein>().FirstOrDefault(predicate);
    }

    public IEnumerable<Protein> Find(IEnumerable<ProteinModel> models, IEnumerable<Protein> cache = null)
    {
        var allIdentifiers = models.Select(model => model.Id).ToArray();

        var cachedEntities = cache?.Where(entity => allIdentifiers.Contains(entity.StableId)).ToArray() ?? Array.Empty<Protein>();

        var cachedIdentifiers = cachedEntities.Select(entity => entity.StableId).ToArray();

        var newIdentifiers = allIdentifiers.Except(cachedIdentifiers).ToArray();

        var newEntities = _dbContext.Set<Protein>().Where(entity => newIdentifiers.Contains(entity.StableId));

        return Enumerable.Concat(cachedEntities, newEntities).ToArray();
    }

    public Protein Create(ProteinModel model, IEnumerable<Transcript> transcriptsCache = null, IEnumerable<Gene> genesCache = null)
    {
        var entity = Convert(model, transcriptsCache, genesCache);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public IEnumerable<Protein> CreateMissing(IEnumerable<ProteinModel> models, IEnumerable<Protein> cache = null, IEnumerable<Transcript> transcriptsCache = null, IEnumerable<Gene> genesCache = null)
    {
        var existingIdentifiers = Find(models, cache).Select(entity => entity.StableId).ToArray();

        var entitiesToAdd = new List<Protein>();

        foreach (var model in models)
        {
            if (existingIdentifiers.Contains(model.Id))
            {
                continue;
            }

            var entity = Find(model);

            if (entity == null)
            {
                entity = Convert(model, transcriptsCache, genesCache);

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


    private Protein Convert(ProteinModel model, IEnumerable<Transcript> transcriptsCache = null, IEnumerable<Gene> genesCache = null)
    {
        var entity = new Protein
        {
            StableId = model.Id,
            Start = model.Start,
            End = model.End,
            Length = model.Length,
            IsCanonical = model.IsCanonical
        };

        if (model.Transcript != null)
        {
            entity.TranscriptId = _transcriptRepository.FindOrCreate(model.Transcript, transcriptsCache, genesCache).Id;
        }

        return entity;
    }
}
