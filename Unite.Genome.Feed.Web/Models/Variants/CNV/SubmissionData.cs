namespace Unite.Genome.Feed.Web.Models.Variants.CNV;

public enum SubmissionType
{
    Default,
    AceSeq
}

public class SubmissionData : Models.Base.SubmissionData<SubmissionType>
{
    public SubmissionData(SubmissionType type) : base(type)
    {
    }
}
