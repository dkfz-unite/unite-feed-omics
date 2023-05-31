using Unite.Data.Entities.Genome.Variants.SSM.Enums;
using Unite.Data.Utilities.Mutations;

namespace Unite.Genome.Feed.Web.Models.Variants.SSM.Mappers;

public class VariantModelMapper
{
    public void Map(in VariantModel source, Data.Models.Variants.SSM.VariantModel target)
    {
        target.Type = MutationTypeDetector.Detect(source.Ref, source.Alt);
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
