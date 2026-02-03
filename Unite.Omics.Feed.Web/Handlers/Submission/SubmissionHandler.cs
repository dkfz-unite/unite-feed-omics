namespace Unite.Omics.Feed.Web.Handlers.Submission;

public enum SubmissionHandlerPriority
{
    Highest = 0,
    High     = 10,
    Normal   = 20,
    Low      = 30,
    Lowest   = 40
}

public interface ISubmissionHandler
{
    SubmissionHandlerPriority Priority { get; }
    void Handle();
}

public class SubmissionHandler
{
    
}