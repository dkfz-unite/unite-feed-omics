using Unite.Data.Entities.Genome.Variants.SSM.Enums;

namespace Unite.Genome.Feed.Data.Models.Variants.SSM;

public class VariantModel : Variants.VariantModel
{
    public SsmType Type { get; set; }
    public string Ref { get; set; }
    public string Alt { get; set; }
}
