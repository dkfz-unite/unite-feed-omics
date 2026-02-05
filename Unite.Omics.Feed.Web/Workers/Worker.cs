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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var reg = _lifetime.ApplicationStarted.Register(() => started.TrySetResult());

        await started.Task.WaitAsync(stoppingToken);
        
        _logger.LogInformation("{WorkerType} worker started", WorkerType);

        stoppingToken.Register(() => _logger.LogInformation("{WorkerType} worker stopped", WorkerType));
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                foreach (var handler in _handlers)
                {
                    handler.Handle();
                }
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
}