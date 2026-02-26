namespace Unite.Omics.Feed.Web.Handlers.Submission;

//TODO: Submission Handlers and corresponding submission models should be moved to Unite.Omics.Feed module(dll)
public abstract class SubmissionHandler(HandlerPriority priority) : Handler, ISubmissionHandler
{
    public HandlerPriority Priority => priority;
    
    public override Task Prepare()
    {
        return Task.CompletedTask;
    }
}