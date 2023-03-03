namespace Unite.Genome.Feed.Web.Models.Variants.SV;

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
