using Unite.Data.Entities.Genome.Variants.SV.Enums;

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

        target.Length = GetLength(source);

        target.FlankingSequenceFrom = source.FlankingSequenceFrom;
        target.FlankingSequenceTo = source.FlankingSequenceFrom;

        target.Type = source.Type.Value;
        target.Inverted = source.Inverted;
    }


    private static int? GetLength(in VariantModel source)
    {
        if (source.Type == null)
        {
            return null;
        }
        else if (source.Type == SvType.ITX || source.Type == SvType.CTX || source.Type == SvType.COM)
        {
            return null;
        }
        else if (source.Chromosome != source.OtherChromosome)
        {
            return null;
        }
        else
        {
            var positions = new int?[] { source.Start, source.End, source.OtherStart, source.OtherEnd };

            return positions.Max() - positions.Min();
        }
    }
}
