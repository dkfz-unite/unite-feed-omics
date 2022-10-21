using Unite.Data.Entities.Genome.Variants.SSM.Enums;

namespace Unite.Genome.Feed.Data.Models.Variants.SSM;

public class VariantModel : Variants.VariantModel
{
    public string Code { get; set; }
    public SsmType Type { get; set; }
    public string ReferenceBase { get; set; }
    public string AlternateBase { get; set; }
}
