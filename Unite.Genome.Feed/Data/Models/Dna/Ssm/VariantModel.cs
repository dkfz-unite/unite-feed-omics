using Unite.Data.Entities.Genome.Analysis.Dna.Ssm.Enums;

namespace Unite.Genome.Feed.Data.Models.Dna.Ssm;

public class VariantModel : Dna.VariantModel
{
    public SsmType Type { get; set; }
    public string Ref { get; set; }
    public string Alt { get; set; }
}
