using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Variants.SV.Enums;

namespace Unite.Genome.Feed.Data.Models.Variants.SV;

public class VariantModel : Variants.VariantModel
{
    public Chromosome? NewChromosome { get; set; }
    public double? NewStart { get; set; }
    public double? NewEnd { get; set; }
    public SvType Type { get; set; }
    public string ReferenceBase { get; set; }
    public string AlternateBase { get; set; }
}
