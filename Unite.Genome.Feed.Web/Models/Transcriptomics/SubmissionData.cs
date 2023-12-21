namespace Unite.Genome.Feed.Web.Models.Transcriptomics;

public enum SubmissionType
{
    /// <summary>
    /// Data in default JSON format.
    /// </summary>
    Default
}

public class SubmissionData : Base.SubmissionData<SubmissionType>
{
    public SubmissionData(SubmissionType type) : base(type)
    {
    }
}
