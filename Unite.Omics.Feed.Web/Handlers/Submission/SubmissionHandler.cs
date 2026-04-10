namespace Unite.Omics.Feed.Web.Handlers.Submission;

//TODO: Submission Handlers and corresponding submission models should be moved to Unite.Omics.Feed module(dll)
public abstract class SubmissionHandler : IHandler
{
    public void Prepare() {}

    public abstract void Handle();
}
