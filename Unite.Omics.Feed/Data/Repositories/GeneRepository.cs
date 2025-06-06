using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Repositories;

public class GeneRepository
{
    private readonly DomainDbContext _dbContext;


    public GeneRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Gene FindOrCreate(GeneModel model, IEnumerable<Gene> cache = null)
    {
        return Find(model, cache) ?? Create(model);
    }

    public Gene Find(GeneModel model, IEnumerable<Gene> cache = null)
    {
        Expression<Func<Gene, bool>> predicate = (entity) => entity.StableId == model.Id;

        return cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<Gene>().FirstOrDefault(predicate);
    }

    public IEnumerable<Gene> Find(IEnumerable<GeneModel> models, IEnumerable<Gene> cache = null)
    {
        var allIdentifiers = models.Select(model => model.Id).ToArray();

        var cachedEntities = cache?.Where(entity => allIdentifiers.Contains(entity.StableId)).ToArray() ?? Array.Empty<Gene>();

        var cachedIdentifiers = cachedEntities.Select(entity => entity.StableId).ToArray();

        var newIdentifiers = allIdentifiers.Except(cachedIdentifiers).ToArray();

        var newEntities = _dbContext.Set<Gene>().Where(entity => newIdentifiers.Contains(entity.StableId));

        return Enumerable.Concat(cachedEntities, newEntities).ToArray();
    }

    public Gene Create(GeneModel model)
    {
        var gene = Convert(model);

        _dbContext.Add(gene);
        _dbContext.SaveChanges();

        return gene;
    }

    public IEnumerable<Gene> CreateMissing(IEnumerable<GeneModel> models, IEnumerable<Gene> cache = null)
    {
        var existingIdentifiers = Find(models, cache).Select(entity => entity.StableId).ToArray();

        var entitiesToAdd = new List<Gene>();

        foreach (var model in models)
        {
            if (existingIdentifiers.Contains(model.Id))
            {
                continue;
            }

            var entity = Find(model);

            if (entity == null)
            {
                entity = Convert(model);

                entitiesToAdd.Add(entity);
            }
        }

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd.ToArray();
    }


    private Gene Convert(GeneModel model)
    {
        var entity = new Gene
        {
            StableId = model.Id,
            Symbol = model.Symbol,
            Description = model.Description,
            Biotype = model.Biotype,
            ChromosomeId = model.Chromosome,
            Start = model.Start,
            End = model.End,
            Strand = model.Strand,
            ExonicLength = model.ExonicLength
        };

        return entity;
    }
}
