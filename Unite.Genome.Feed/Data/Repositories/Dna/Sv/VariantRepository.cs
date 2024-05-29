using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Sv;
using Unite.Genome.Feed.Data.Models.Dna.Sv;

namespace Unite.Genome.Feed.Data.Repositories.Dna.Sv;

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
