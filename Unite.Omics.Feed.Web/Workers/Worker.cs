using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Handlers;

namespace Unite.Omics.Feed.Web.Workers;

public abstract class Worker<THandlerInterface>: BackgroundService
    where THandlerInterface: IHandler
{
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly THandlerInterface[] _handlers;
    
    protected virtual int CyclePauseTimeMs { get; } = 10000;
    protected abstract string WorkerType { get; }

    protected IEnumerable<THandlerInterface> Handlers => _handlers;
    protected ILogger Logger => _logger;

    public Worker(IEnumerable<THandlerInterface> handlers,
        IHostApplicationLifetime lifetime,
        ILogger<SubmissionsWorker> logger)
    {
        _logger = logger;
        _lifetime = lifetime;
        _handlers = handlers
            .OrderBy(h => h.Priority)
            .ToArray();
    }
    protected abstract Task ScheduleHandlers(CancellationToken stoppingToken);
    protected abstract Task PrepareHandlers(CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var reg = _lifetime.ApplicationStarted.Register(() => started.TrySetResult());

        await started.Task.WaitAsync(stoppingToken);
        
        _logger.LogInformation("{WorkerType} worker started", WorkerType);
        
        try
        {
            await PrepareHandlers(stoppingToken);
        }
        catch (Exception exception)
        {
            _logger.LogError("{error}", exception.GetShortMessage());
        }

        stoppingToken.Register(() => _logger.LogInformation("{WorkerType} worker stopped", WorkerType));
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ScheduleHandlers(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{WorkerType} processing failed", WorkerType);
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