namespace Unite.Genome.Annotations.Data.Models;

public class AnnotationsModel
{
    public MutationModel Mutation { get; set; }

    public AffectedTranscriptModel[] AffectedTranscripts { get; set; }

}
