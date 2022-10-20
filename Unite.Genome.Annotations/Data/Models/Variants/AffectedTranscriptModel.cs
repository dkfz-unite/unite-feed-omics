namespace Unite.Genome.Annotations.Data.Models.Variants;

public class AffectedTranscriptModel
{
    public int? CDNAStart { get; set; }
    public int? CDNAEnd { get; set; }
    public int? CDSStart { get; set; }
    public int? CDSEnd { get; set; }
    public int? ProteinStart { get; set; }
    public int? ProteinEnd { get; set; }
    public string AminoAcidChange { get; set; }
    public string CodonChange { get; set; }
    public int? OverlapBpNumber { get; set; }
    public int? OverlapPercentage { get; set; }
    public int? Distance { get; set; }
    public string[] Consequences { get; set; }

    public VariantModel Variant { get; set; }
    public TranscriptModel Transcript { get; set; }
}
