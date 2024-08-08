using Unite.Data.Entities.Genome.Analysis.Dna.Ssm.Enums;

namespace Unite.Genome.Feed.Data.Models.Dna.Ssm;

public class VariantModel : Dna.VariantModel
{
    public SsmType Type;
    public string Ref;
    public string Alt;
}
