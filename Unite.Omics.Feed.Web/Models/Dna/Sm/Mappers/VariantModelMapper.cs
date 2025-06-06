using Unite.Data.Entities.Omics.Analysis.Dna.Sm.Enums;
using Unite.Data.Helpers.Omics.Dna.Sm;

namespace Unite.Omics.Feed.Web.Models.Dna.Sm.Mappers;

public class VariantModelMapper
{
    public void Map(in VariantModel source, Data.Models.Dna.Sm.VariantModel target)
    {
        target.Type = TypeDetector.Detect(source.Ref, source.Alt);
        target.Chromosome = source.Chromosome.Value;
        target.Start = PositionParser.Parse(source.Position).Start;
        target.End = PositionParser.Parse(source.Position).End;
        target.Ref = source.Ref;
        target.Alt = source.Alt;

        if (target.Type == SmType.INS)
        {
            target.Length = target.Alt.Length;
        }
        else if (target.Type == SmType.DEL)
        {
            target.Length = target.Ref.Length;
        }
        else
        {
            target.Length = target.End - target.Start + 1;
        }
    }
}
