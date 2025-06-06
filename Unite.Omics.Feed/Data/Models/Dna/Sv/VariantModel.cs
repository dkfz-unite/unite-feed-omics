using Unite.Data.Entities.Omics.Analysis.Dna.Sv.Enums;
using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Data.Models.Dna.Sv;

public class VariantModel : Dna.VariantModel
{
    public Chromosome OtherChromosome;
    public int OtherStart;
    public int OtherEnd;
    public SvType Type;
    public bool? Inverted;
    public string FlankingSequenceFrom;
    public string FlankingSequenceTo;
}
