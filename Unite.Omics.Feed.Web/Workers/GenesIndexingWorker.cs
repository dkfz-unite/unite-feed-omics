using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers.Indexing;

namespace Unite.Omics.Feed.Web.Workers;

public class GenesIndexingWorker : BackgroundService
{
    private readonly GenesIndexingOptions _options;
    private readonly GenesIndexingHandler _handler;
    private readonly ILogger _logger;


    public GenesIndexingWorker(
        GenesIndexingOptions options,
        GenesIndexingHandler handler,
        ILogger<GenesIndexingWorker> logger)
    {
        _options = options;
        _handler = handler;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Genes indexing worker started");

        stoppingToken.Register(() => _logger.LogInformation("Genes indexing worker stopped"));

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, stoppingToken);

        try
        {
            await _handler.Prepare();
        }
        catch (Exception exception)
        {
            _logger.LogError("{error}", exception.GetShortMessage());
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _handler.Handle(_options.BucketSize);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Genes indexing process failed");
            }
            finally
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
