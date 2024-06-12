using Unite.Data.Entities.Genome.Analysis.Dna.Sv.Enums;
using Unite.Data.Entities.Genome.Enums;

namespace Unite.Genome.Feed.Data.Models.Dna.Sv;

public class VariantModel : Dna.VariantModel
{
    public Chromosome OtherChromosome { get; set; }
    public int OtherStart { get; set; }
    public int OtherEnd { get; set; }
    public SvType Type { get; set; }
    public bool? Inverted { get; set; }
    public string FlankingSequenceFrom { get; set; }
    public string FlankingSequenceTo { get; set; }
}
