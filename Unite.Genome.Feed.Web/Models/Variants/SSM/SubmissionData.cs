namespace Unite.Genome.Feed.Web.Models.Variants.SSM;

public enum SubmissionType
{
    Default
}

public class SubmissionData : Models.Base.SubmissionData<SubmissionType>
{
    public SubmissionData(SubmissionType type) : base(type)
    {
    }
}
