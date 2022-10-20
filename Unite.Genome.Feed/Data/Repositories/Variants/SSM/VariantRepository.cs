using Unite.Data.Entities.Genome.Variants.SSM;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Variants.SSM;

namespace Unite.Genome.Feed.Data.Repositories.Variants.SSM;

internal class VariantRepository : VariantRepository<Variant, VariantModel>
{
    public VariantRepository(DomainDbContext dbContext) : base(dbContext)
    {
    }

    public override Variant Find(VariantModel model)
    {
        var entity = _dbContext.Set<Variant>()
            .FirstOrDefault(entity =>
                entity.Code == model.Code
            );

        return entity;
    }

    protected override void Map(in VariantModel model, ref Variant entity)
    {
        base.Map(model, ref entity);

        entity.Code = model.Code;
        entity.TypeId = model.Type;
        entity.ReferenceBase = model.ReferenceBase;
        entity.AlternateBase = model.AlternateBase;
    }
}
