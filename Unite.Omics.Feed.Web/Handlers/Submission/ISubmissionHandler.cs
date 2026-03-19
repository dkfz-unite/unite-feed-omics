namespace Unite.Omics.Feed.Web.Handlers.Submission;

public interface ISubmissionHandler : IHandler
{
    HandlerPriority Priority { get; }
}