using System.Linq.Expressions;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Variants;

namespace Unite.Genome.Feed.Data.Repositories.Variants;

public abstract class VariantRepository<TEntity, TModel>
    where TEntity : Variant
    where TModel : VariantModel
{
    protected readonly DomainDbContext _dbContext;


    protected VariantRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public virtual TEntity Find(TModel model, IEnumerable<TEntity> cache = null)
    {
        var predicate = model.Id != null ? GetIdPredicate(model.Id.Value) : GetModelPredicate(model);

        return cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<TEntity>().FirstOrDefault(predicate);
    }

    public virtual TEntity Find(VariantModel model, IEnumerable<TEntity> cache = null)
    {
        var predicate = GetModelPredicate(model);

        return cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<TEntity>().FirstOrDefault(predicate);
    }

    public virtual TEntity[] Find(IEnumerable<VariantModel> models)
    {
        var predicate = GetModelsPredicate(models);

        return _dbContext.Set<TEntity>().Where(predicate).ToArray();
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

    protected virtual Expression<Func<TEntity, bool>> GetModelPredicate(VariantModel model)
    {
        return (entity) => entity.Id == model.Id;
    }

    protected virtual Expression<Func<TEntity, bool>> GetModelsPredicate(IEnumerable<VariantModel> models)
    {
        var ids = models.Select(model => model.Id);

        return (entity) => ids.Contains(entity.Id);
    }

    protected virtual Expression<Func<TEntity, bool>> GetIdPredicate(long id)
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
