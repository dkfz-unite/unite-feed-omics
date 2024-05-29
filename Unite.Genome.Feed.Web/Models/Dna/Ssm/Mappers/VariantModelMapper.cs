using Unite.Data.Entities.Genome.Analysis.Dna.Ssm.Enums;
using Unite.Data.Helpers.Genome.Dna.Ssm;

namespace Unite.Genome.Feed.Web.Models.Dna.Ssm.Mappers;

public class VariantModelMapper
{
    public void Map(in VariantModel source, Data.Models.Dna.Ssm.VariantModel target)
    {
        target.Type = TypeDetector.Detect(source.Ref, source.Alt);
        target.Chromosome = source.Chromosome.Value;
        target.Start = PositionParser.Parse(source.Position).Start;
        target.End = PositionParser.Parse(source.Position).End;
        target.Ref = source.Ref;
        target.Alt = source.Alt;

        if (target.Type == SsmType.INS)
        {
            target.Length = target.Alt.Length;
        }
        else if (target.Type == SsmType.DEL)
        {
            target.Length = target.Ref.Length;
        }
        else
        {
            target.Length = target.End - target.Start + 1;
        }
    }
}
