using Unite.Omics.Feed.Web.Handlers.Submission;

namespace Unite.Omics.Feed.Web.Workers;

public class SubmissionsWorker(
    IEnumerable<ISubmissionHandler> handlers,
    IHostApplicationLifetime lifetime,
    ILogger<SubmissionsWorker> logger)
    : Worker<ISubmissionHandler>(handlers, lifetime, logger)
{
    protected override string WorkerType => "Submissions";
}
