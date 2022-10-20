using Unite.Data.Entities.Genome.Variants.SV;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Variants.SV;

namespace Unite.Genome.Feed.Data.Repositories.Variants.SV;

internal class VariantRepository : VariantRepository<Variant, VariantModel>
{
    public VariantRepository(DomainDbContext dbContext) : base(dbContext)
    {
    }

    public override Variant Find(VariantModel model)
    {
        var entity = _dbContext.Set<Variant>()
            .FirstOrDefault(entity =>
                entity.ChromosomeId == model.Chromosome &&
                entity.Start == model.Start &&
                entity.End == model.End &&
                entity.NewChromosomeId == model.NewChromosome &&
                entity.NewStart == model.NewStart &&
                entity.NewEnd == model.NewEnd &&
                entity.TypeId == model.Type &&
                entity.ReferenceBase == model.ReferenceBase &&
                entity.AlternateBase == model.AlternateBase
            );

        return entity;
    }

    protected override void Map(in VariantModel model, ref Variant entity)
    {
        base.Map(model, ref entity);

        entity.NewChromosomeId = model.NewChromosome;
        entity.NewStart = model.NewStart;
        entity.NewEnd = model.NewEnd;
        entity.TypeId = model.Type;
        entity.ReferenceBase = model.ReferenceBase;
        entity.AlternateBase = model.AlternateBase;
    }
}
