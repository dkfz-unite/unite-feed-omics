using Unite.Data.Entities.Omics.Analysis.Dna.Sm.Enums;

namespace Unite.Omics.Feed.Data.Models.Dna.Sm;

public class VariantModel : Dna.VariantModel
{
    public SmType Type;
    public string Ref;
    public string Alt;
}
