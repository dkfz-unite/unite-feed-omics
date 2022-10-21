namespace Unite.Genome.Feed.Web.Models.Variants.SV.Mappers;

public class VariantModelMapper
{
    public void Map(in VariantModel source, Data.Models.Variants.SV.VariantModel target)
    {
        target.Chromosome = source.Chromosome.Value;
        target.Start = source.Start.Value;
        target.End = source.End.Value;

        target.NewChromosome = source.NewChromosome;
        target.NewStart = source.NewStart;
        target.NewEnd = source.NewEnd;

        target.ReferenceBase = source.Ref;
        target.AlternateBase = source.Alt;

        target.Type = source.Type.Value;
    }
}
