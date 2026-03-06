using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Handlers;

namespace Unite.Omics.Feed.Web.Workers;

public abstract class Worker<THandlerInterface> : BackgroundService
    where THandlerInterface : IHandler
{
    private THandlerInterface[] _handlers;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<SubmissionsWorker> _logger;

    protected Worker(IEnumerable<THandlerInterface> handlers,
        IHostApplicationLifetime lifetime,
        ILogger<SubmissionsWorker> logger)
    {
        _lifetime = lifetime;
        _logger = logger;
        _handlers = handlers.ToArray();
    }

    protected virtual int CyclePauseTimeMs { get; } = 10000;
    protected abstract string WorkerType { get; }

    protected THandlerInterface[] Handlers => _handlers;
    protected ILogger Logger => _logger;

    protected abstract Task ScheduleHandlers(CancellationToken stoppingToken);
    protected abstract Task<THandlerInterface[]> PrepareHandlers(CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var reg = _lifetime.ApplicationStarted.Register(() => started.TrySetResult());

        await started.Task.WaitAsync(stoppingToken);
        
        _logger.LogInformation("{WorkerType} worker started", WorkerType);
        
        try
        {
            _handlers = await PrepareHandlers(stoppingToken);
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