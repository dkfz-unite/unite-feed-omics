using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers.Indexing;

namespace Unite.Genome.Feed.Web.HostedServices;

public class GenesIndexingHostedService : BackgroundService
{
    private readonly GenesIndexingOptions _options;
    private readonly GenesIndexingHandler _handler;
    private readonly ILogger _logger;


    public GenesIndexingHostedService(
        GenesIndexingOptions options,
        GenesIndexingHandler handler,
        ILogger<GenesIndexingHostedService> logger)
    {
        _options = options;
        _handler = handler;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Genes indexing service started");

        stoppingToken.Register(() => _logger.LogInformation("Genes indexing service stopped"));

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
                _logger.LogError("{error}", exception.GetShortMessage());
            }
            finally
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
