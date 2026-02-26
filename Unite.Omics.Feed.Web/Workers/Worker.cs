using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Handlers;

namespace Unite.Omics.Feed.Web.Workers;

public abstract class Worker<THandlerInterface>(
    IEnumerable<THandlerInterface> handlers,
    IHostApplicationLifetime lifetime,
    ILogger<SubmissionsWorker> logger)
    : BackgroundService
    where THandlerInterface : IHandler
{
    private THandlerInterface[] _handlers = handlers.ToArray();
    protected virtual int CyclePauseTimeMs { get; } = 10000;
    protected abstract string WorkerType { get; }

    protected THandlerInterface[] Handlers => _handlers;
    protected ILogger Logger => logger;

    protected abstract Task ScheduleHandlers(CancellationToken stoppingToken);
    protected abstract Task<THandlerInterface[]> PrepareHandlers(CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var reg = lifetime.ApplicationStarted.Register(() => started.TrySetResult());

        await started.Task.WaitAsync(stoppingToken);
        
        logger.LogInformation("{WorkerType} worker started", WorkerType);
        
        try
        {
            _handlers = await PrepareHandlers(stoppingToken);
        }
        catch (Exception exception)
        {
            logger.LogError("{error}", exception.GetShortMessage());
        }

        stoppingToken.Register(() => logger.LogInformation("{WorkerType} worker stopped", WorkerType));
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ScheduleHandlers(stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "{WorkerType} processing failed", WorkerType);
            }
            finally
            {
                await Task.Delay(CyclePauseTimeMs, stoppingToken);
            }
        }
    }

    protected virtual async Task RunHandler(THandlerInterface handler, CancellationToken stoppingToken)
    {
        try
        {
            await handler.Handle();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "{WorkerType} worker: handler failed {FullName}", WorkerType, handler.GetType().FullName);
        }
    }
}