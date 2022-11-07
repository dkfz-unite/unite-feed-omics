using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Variants.SV.Enums;

namespace Unite.Genome.Feed.Data.Models.Variants.SV;

public class VariantModel : Variants.VariantModel
{
    public Chromosome OtherChromosome { get; set; }
    public int OtherStart { get; set; }
    public int OtherEnd { get; set; }
    public SvType Type { get; set; }
    public bool? Inverted { get; set; }
    public string FlankingSequenceFrom { get; set; }
    public string FlankingSequenceTo { get; set; }
}
