using System.Linq.Expressions;
using Unite.Data.Entities.Genome.Variants;
using Unite.Data.Services;
using Unite.Genome.Annotations.Data.Models.Variants;

namespace Unite.Genome.Annotations.Data.Repositories;

internal class VariantRepository<TVariant> where TVariant : Variant
{
    private readonly DomainDbContext _dbContext;


    public VariantRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public TVariant Find(VariantModel model, IEnumerable<TVariant> cache = null)
    {
        Expression<Func<TVariant, bool>> predicate = (entity) =>
            entity.Id == model.Id;

        var entity = cache?.FirstOrDefault(predicate.Compile()) ?? _dbContext.Set<TVariant>().FirstOrDefault(predicate);

        return entity;
    }

    public TVariant[] Find(IEnumerable<VariantModel> models)
    {
        var variantIds = models.Select(model => model.Id);

        var entities = _dbContext.Set<TVariant>()
            .Where(entity => variantIds.Contains(entity.Id))
            .ToArray();

        return entities;
    }
}
