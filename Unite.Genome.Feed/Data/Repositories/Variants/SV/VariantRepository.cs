using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Variants.SV;
using Unite.Genome.Feed.Data.Models.Variants.SV;

namespace Unite.Genome.Feed.Data.Repositories.Variants.SV;

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
            entity.OtherChromosomeId == model.OtherChromosome &&
            entity.OtherStart == model.OtherStart &&
            entity.OtherEnd == model.OtherEnd &&
            entity.TypeId == model.Type &&
            entity.Inverted == model.Inverted &&
            entity.FlankingSequenceFrom == model.FlankingSequenceFrom &&
            entity.FlankingSequenceTo == model.FlankingSequenceTo;
    }

    protected override void Map(in VariantModel model, ref Variant entity)
    {
        base.Map(model, ref entity);

        entity.OtherChromosomeId = model.OtherChromosome;
        entity.OtherStart = model.OtherStart;
        entity.OtherEnd = model.OtherEnd;
        entity.TypeId = model.Type;
        entity.Inverted = model.Inverted;
        entity.FlankingSequenceFrom = model.FlankingSequenceFrom;
        entity.FlankingSequenceTo = model.FlankingSequenceTo;
    }
}
