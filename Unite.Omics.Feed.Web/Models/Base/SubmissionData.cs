namespace Unite.Omics.Feed.Web.Models.Base;

public abstract class SubmissionData<T> 
    where T : struct, Enum
{
    public T Type { get; set; }

    protected SubmissionData(T type)
    {
	    Type = type;
    }
}
