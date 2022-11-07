namespace Unite.Genome.Feed.Web.Models.Variants.SV.Mappers;

public class VariantModelMapper
{
    public void Map(in VariantModel source, Data.Models.Variants.SV.VariantModel target)
    {
        target.Chromosome = source.Chromosome.Value;
        target.Start = source.Start.Value;
        target.End = source.End.Value;

        target.OtherChromosome = source.OtherChromosome.Value;
        target.OtherStart = source.OtherStart.Value;
        target.OtherEnd = source.OtherEnd.Value;

        target.FlankingSequenceFrom = source.FlankingSequenceFrom;
        target.FlankingSequenceTo = source.FlankingSequenceFrom;

        target.Type = source.Type.Value;
        target.Inverted = source.Inverted;
    }
}
