using Unite.Data.Entities.Genome.Analysis.Dna.Sm.Enums;

namespace Unite.Genome.Feed.Data.Models.Dna.Sm;

public class VariantModel : Dna.VariantModel
{
    public SmType Type;
    public string Ref;
    public string Alt;
}
