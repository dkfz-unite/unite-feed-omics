using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers.Indexing;

namespace Unite.Genome.Feed.Web.HostedServices;

public class VariantsIndexingHostedService : BackgroundService
{
    private readonly VariantsIndexingOptions _options;
    private readonly SsmsIndexingHandler _ssmsIndexingHandler;
    private readonly CnvsIndexingHandler _cnvsVariantsIndexingHandler;
    private readonly SvsIndexingHandler _svsIndexingHandler;
    private readonly ILogger _logger;


    public VariantsIndexingHostedService(
        VariantsIndexingOptions options,
        SsmsIndexingHandler ssmsIndexingHandler,
        CnvsIndexingHandler cnvsIndexingHandler,
        SvsIndexingHandler svsIndexingHandler,
        ILogger<VariantsIndexingHostedService> logger)
    {
        _options = options;
        _ssmsIndexingHandler = ssmsIndexingHandler;
        _cnvsVariantsIndexingHandler = cnvsIndexingHandler;
        _svsIndexingHandler = svsIndexingHandler;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Variants indexing service started");

        stoppingToken.Register(() => _logger.LogInformation("Variants indexing service stopped"));

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, stoppingToken);

        try
        {
            _ssmsIndexingHandler.Prepare();
            _cnvsVariantsIndexingHandler.Prepare();
            _svsIndexingHandler.Prepare();
        }
        catch (Exception exception)
        {
            _logger.LogError("{error}", exception.GetShortMessage());
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _ssmsIndexingHandler.Handle(_options.SsmBucketSize);
                _cnvsVariantsIndexingHandler.Handle(_options.CnvBucketSize);
                _svsIndexingHandler.Handle(_options.SvBucketSize);
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
