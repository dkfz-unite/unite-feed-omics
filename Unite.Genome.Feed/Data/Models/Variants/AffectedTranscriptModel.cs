namespace Unite.Genome.Feed.Data.Models.Variants;

public class AffectedTranscriptModel
{
    public int? CDNAStart;
    public int? CDNAEnd;
    public int? CDSStart;
    public int? CDSEnd;
    public int? ProteinStart;
    public int? ProteinEnd;
    public string AminoAcidChange;
    public string CodonChange;
    public int? OverlapBpNumber;
    public double? OverlapPercentage;
    public int? Distance;
    public string[] Consequences;

    public VariantModel Variant;
    public GeneModel Gene;
    public TranscriptModel Transcript;
    public ProteinModel Protein;
}
