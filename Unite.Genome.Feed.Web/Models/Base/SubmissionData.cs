namespace Unite.Genome.Feed.Web.Models.Base;

public abstract class SubmissionData<T> where T : Enum
{
	public T Type { get; set; }

    protected SubmissionData(T type)
    {
		Type = type;
    }
}
