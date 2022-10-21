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


    public TVariant Find(VariantModel model)
    {
        var entity = _dbContext.Set<TVariant>()
            .FirstOrDefault(entity =>
                entity.Id == model.Id
            );

        return entity;
    }
}
