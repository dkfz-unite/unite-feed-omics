namespace Unite.Omics.Feed.Web.Handlers.Submission;

public abstract class SubmissionHandler(HandlerPriority priority) : Handler(priority), ISubmissionHandler;