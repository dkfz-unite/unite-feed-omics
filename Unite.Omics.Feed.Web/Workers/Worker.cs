using Unite.Omics.Feed.Web.Handlers;

namespace Unite.Omics.Feed.Web.Workers;

public abstract class Worker : BackgroundService
{
    protected readonly ILogger _logger;
    protected IHandler[] _handlers;
    protected virtual int CycleIntervalMs => 10000;


    protected Worker(ILogger logger)
    {
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, stoppingToken);

        _logger.LogInformation("Worker started");

        stoppingToken.Register(() => _logger.LogInformation("Worker stopped"));

        PrepareHandlers(_handlers, stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                RunHandlers(_handlers, stoppingToken);
            }
            finally
            {
                await Task.Delay(CycleIntervalMs, stoppingToken);
            }
        }
    }

    protected virtual void PrepareHandlers(IHandler[] handlers, CancellationToken stoppingToken)
    {
        foreach (var handler in handlers)
        {
            if (stoppingToken.IsCancellationRequested)
                return;

            PrepareHandler(handler);
        }
    }

    protected virtual void PrepareHandler(IHandler handler)
    {
        try
        {
            handler.Prepare();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{handler} preparing failed", handler.GetType().Name);
        }
    }
    
    protected virtual void RunHandlers(IHandler[] handlers, CancellationToken stoppingToken)
    {
        foreach (var handler in handlers)
        {
            if (stoppingToken.IsCancellationRequested)
                return;

            RunHandler(handler);
        }
    }

    protected virtual void RunHandler(IHandler handler)
    {
        try
        {
            handler.Handle();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{handler} failed", handler.GetType().Name);
        }
    }
}
