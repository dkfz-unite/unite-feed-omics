namespace Unite.Omics.Feed.Web.Handlers.Submission;

//TODO: Submission Handlers and corresponding submission models should be moved to Unite.Omics.Feed module(dll)
public abstract class SubmissionHandler : Handler, ISubmissionHandler
{
    private readonly HandlerPriority _priority;

    protected SubmissionHandler(HandlerPriority priority)
    {
        _priority = priority;
    }

    public HandlerPriority Priority => _priority;
}