using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using Unite.Genome.Feed.Data.Models.Dna.Ssm;

namespace Unite.Genome.Feed.Data.Repositories.Dna.Ssm;

public class VariantRepository : VariantRepository<Variant, VariantModel>
{
    public VariantRepository(DomainDbContext dbContext) : base(dbContext)
    {
    }

    protected override Expression<Func<Variant, bool>> GetModelPredicate(VariantModel model)
    {
        return (entity) =>
            entity.ChromosomeId == model.Chromosome &&
            entity.Start == model.Start &&
            entity.End == model.End &&
            entity.Ref == model.Ref &&
            entity.Alt == model.Alt;
    }

    protected override void Map(in VariantModel model, ref Variant entity)
    {
        base.Map(model, ref entity);

        entity.TypeId = model.Type;
        entity.Ref = model.Ref;
        entity.Alt = model.Alt;
    }
}
