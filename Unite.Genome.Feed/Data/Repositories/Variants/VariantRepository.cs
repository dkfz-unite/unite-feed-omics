using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Variants;

namespace Unite.Genome.Feed.Data.Repositories.Variants;

internal abstract class VariantRepository<TEntity, TModel>
    where TEntity : Variant
    where TModel : VariantModel
{
    protected readonly DomainDbContext _dbContext;


    protected VariantRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public abstract TEntity Find(TModel model);

    public virtual TEntity Create(TModel model)
    {
        var entity = Activator.CreateInstance<TEntity>();

        Map(model, ref entity);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public virtual TEntity FindOrCreate(TModel model)
    {
        return Find(model) ?? Create(model);
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


    protected virtual void Map(in TModel model, ref TEntity entity)
    {
        entity.ChromosomeId = model.Chromosome;
        entity.Start = model.Start;
        entity.End = model.End;
        entity.Length = model.Length;
    }
}
