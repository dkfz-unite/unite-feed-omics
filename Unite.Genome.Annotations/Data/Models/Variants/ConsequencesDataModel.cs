namespace Unite.Genome.Annotations.Data.Models.Variants;

public class ConsequencesDataModel
{
    public VariantModel Variant { get; set; }

    public AffectedTranscriptModel[] AffectedTranscripts { get; set; }
}
