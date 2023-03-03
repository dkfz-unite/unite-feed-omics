using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers.Indexing;

namespace Unite.Genome.Feed.Web.HostedServices;

public class VariantsIndexingHostedService : BackgroundService
{
    private readonly VariantsIndexingOptions _options;
    private readonly MutationsIndexingHandler _mutationsIndexingHandler;
    private readonly CopyNumberVariantsIndexingHandler _copyNumberVariantsIndexingHandler;
    private readonly StructuralVariantsIndexingHandler _structuralVariantsIndexingHandler;
    private readonly ILogger _logger;


    public VariantsIndexingHostedService(
        VariantsIndexingOptions options,
        MutationsIndexingHandler mutationsIndexingHandler,
        CopyNumberVariantsIndexingHandler copyNumberVariantsIndexingHandler,
        StructuralVariantsIndexingHandler structuralVariantsIndexingHandler,
        ILogger<VariantsIndexingHostedService> logger)
    {
        _options = options;
        _mutationsIndexingHandler = mutationsIndexingHandler;
        _copyNumberVariantsIndexingHandler = copyNumberVariantsIndexingHandler;
        _structuralVariantsIndexingHandler = structuralVariantsIndexingHandler;
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
            _mutationsIndexingHandler.Prepare();
            _copyNumberVariantsIndexingHandler.Prepare();
            _structuralVariantsIndexingHandler.Prepare();
        }
        catch (Exception exception)
        {
            LogError(exception);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _mutationsIndexingHandler.Handle(_options.SsmBucketSize);
                _copyNumberVariantsIndexingHandler.Handle(_options.CnvBucketSize);
                _structuralVariantsIndexingHandler.Handle(_options.SvBucketSize);
            }
            catch (Exception exception)
            {
                LogError(exception);
            }
            finally
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }

    private void LogError(Exception exception)
    {
        _logger.LogError(exception.Message);

        if (exception.InnerException != null)
        {
            LogError(exception.InnerException);
        }
    }
}
