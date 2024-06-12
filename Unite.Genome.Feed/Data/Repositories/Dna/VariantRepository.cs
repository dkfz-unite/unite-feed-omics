using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna;
using Unite.Genome.Feed.Data.Models.Dna;

namespace Unite.Genome.Feed.Data.Repositories.Dna;

public abstract class VariantRepository<TEntity, TModel>
    where TEntity : Variant
    where TModel : VariantModel
{
    protected readonly DomainDbContext _dbContext;


    public VariantRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public virtual TEntity Find(TModel model, IEnumerable<TEntity> cache = null)
    {
        var predicate = GetModelPredicate(model);

        return cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<TEntity>().FirstOrDefault(predicate);
    }

    public virtual TEntity Find(int id, IEnumerable<TEntity> cache = null)
    {
        var predicate = GetIdPredicate(id);

        return cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<TEntity>().FirstOrDefault(predicate);
    }

    public virtual TEntity[] Find(IEnumerable<int> ids)
    {
        return _dbContext.Set<TEntity>().Where(entity => ids.Contains(entity.Id)).ToArray();
    }

    public virtual TEntity Create(TModel model)
    {
        var entity = Activator.CreateInstance<TEntity>();

        Map(model, ref entity);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public virtual TEntity FindOrCreate(TModel model, IEnumerable<TEntity> cache = null)
    {
        return Find(model, cache) ?? Create(model);
    }

    public virtual IEnumerable<TEntity> CreateMissing(IEnumerable<TModel> models)
    {
        var entitiesToAdd = new List<TEntity>();

        foreach (var model in models)
        {
            var entity = Find(model);

            if (entity == null)
            {
                entity = Activator.CreateInstance<TEntity>();

                Map(model, ref entity);

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


    protected abstract Expression<Func<TEntity, bool>> GetModelPredicate(TModel model);

    protected virtual Expression<Func<TEntity, bool>> GetIdPredicate(int id)
    {
        return (entity) => entity.Id == id;
    }

    protected virtual void Map(in TModel model, ref TEntity entity)
    {
        entity.ChromosomeId = model.Chromosome;
        entity.Start = model.Start;
        entity.End = model.End;
        entity.Length = model.Length;
    }
}
