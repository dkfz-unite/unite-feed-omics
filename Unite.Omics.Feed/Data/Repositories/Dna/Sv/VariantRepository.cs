using System.Linq.Expressions;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv.Enums;
using Unite.Data.Entities.Omics.Enums;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Data.Helpers;
using Unite.Omics.Feed.Data.Models.Dna.Sv;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Sv;

public class VariantRepository : VariantRepository<Variant, VariantModel>
{
    public VariantRepository(DomainDbContext dbContext, IGenomeOptions genomeOptions) : base(dbContext, genomeOptions)
    {
    }

    protected override Expression<Func<Variant, bool>> GetModelPredicate(VariantModel model)
    {
        return (entity) =>
            entity.ChromosomeId == model.Chromosome &&
            entity.ChromosomeArmId == model.ChromosomeArm &&
            entity.Start == model.Start &&
            entity.End == model.End &&
            entity.OtherChromosomeId == model.OtherChromosome &&
            entity.OtherChromosomeArmId == model.OtherChromosomeArm &&
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
        entity.OtherChromosomeArmId = ChromosomeArmDetector.Detect(model.OtherChromosome, model.OtherStart, model.OtherEnd, _genomeOptions.Build);
        entity.OtherStart = model.OtherStart;
        entity.OtherEnd = model.OtherEnd;
        entity.TypeId = model.Type;
        entity.Inverted = model.Inverted;
        entity.FlankingSequenceFrom = model.FlankingSequenceFrom;
        entity.FlankingSequenceTo = model.FlankingSequenceTo;
    }
}
