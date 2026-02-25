using Unite.Omics.Feed.Web.Handlers;
using Unite.Omics.Feed.Web.Handlers.Submission;

namespace Unite.Omics.Feed.Web.Workers;

public class SubmissionsWorker(
    IEnumerable<ISubmissionHandler> handlers,
    IHostApplicationLifetime lifetime,
    ILogger<SubmissionsWorker> logger)
    : Worker<ISubmissionHandler>(handlers, lifetime, logger)
{
    protected override string WorkerType => "Submissions";
    
    protected override Task PrepareHandlers(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
    
    protected override async Task ScheduleHandlers(CancellationToken stoppingToken)
    {
        foreach (var handler in Handlers)
        {
            await RunHandler(handler, stoppingToken);
        }
    }
}
