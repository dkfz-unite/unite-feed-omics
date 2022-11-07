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
        target.ReferenceBase = source.Ref;
        target.AlternateBase = source.Alt;

    }
}
