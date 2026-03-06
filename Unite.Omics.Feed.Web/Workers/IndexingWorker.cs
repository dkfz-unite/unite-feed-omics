using Unite.Omics.Feed.Web.Handlers.Indexing;

namespace Unite.Omics.Feed.Web.Workers;

public class IndexingWorker : Worker<IIndexingHandler>
{
    public IndexingWorker(IEnumerable<IIndexingHandler> handlers,
        IHostApplicationLifetime lifetime,
        ILogger<SubmissionsWorker> logger) : base(handlers, lifetime, logger)
    {
    }

    protected override string WorkerType => "Indices";
    
    protected override async Task<IIndexingHandler[]> PrepareHandlers(CancellationToken stoppingToken)
    {
        foreach (var handler in Handlers)
        {
            await handler.Prepare();
        }

        return Handlers;
    }
    
    protected override async Task ScheduleHandlers(CancellationToken stoppingToken)
    {
        IList<Task> handlerTasks = new List<Task>();
        
        foreach (var handler in Handlers)
        {
            handlerTasks.Add(Task.Run(async () =>
            {
                await RunHandler(handler, stoppingToken);
            }, stoppingToken));
        }
        
        await Task.WhenAll(handlerTasks);
    }
}