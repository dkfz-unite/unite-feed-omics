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
                entity.OtherChromosomeId == model.OtherChromosome &&
                entity.OtherStart == model.OtherStart &&
                entity.OtherEnd == model.OtherEnd &&
                entity.TypeId == model.Type &&
                entity.Inverted == model.Inverted &&
                entity.FlankingSequenceFrom == model.FlankingSequenceFrom &&
                entity.FlankingSequenceTo == model.FlankingSequenceTo
            );

        return entity;
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
