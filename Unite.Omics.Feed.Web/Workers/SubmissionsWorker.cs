using Unite.Omics.Feed.Web.Handlers.Submission;

namespace Unite.Omics.Feed.Web.Workers;

public class SubmissionsWorker : Worker<ISubmissionHandler>
{
    public SubmissionsWorker(IEnumerable<ISubmissionHandler> handlers,
        IHostApplicationLifetime lifetime,
        ILogger<SubmissionsWorker> logger) : base(handlers, lifetime, logger)
    {
    }

    protected override string WorkerType => "Submissions";
    
    protected override Task<ISubmissionHandler[]> PrepareHandlers(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
            {
                return Handlers
                    .OrderBy(h => h.Priority)
                    .ToArray();
            }, stoppingToken);
    }
    
    protected override async Task ScheduleHandlers(CancellationToken stoppingToken)
    {
        foreach (var handler in Handlers)
        {
            await RunHandler(handler, stoppingToken);
        }
    }
}
