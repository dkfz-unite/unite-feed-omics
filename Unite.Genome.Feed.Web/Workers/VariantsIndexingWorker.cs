using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers.Indexing;

namespace Unite.Genome.Feed.Web.Workers;

public class VariantsIndexingWorker : BackgroundService
{
    private readonly VariantsIndexingOptions _options;
    private readonly SmsIndexingHandler _smsIndexingHandler;
    private readonly CnvsIndexingHandler _cnvsIndexingHandler;
    private readonly SvsIndexingHandler _svsIndexingHandler;
    private readonly ILogger _logger;


    public VariantsIndexingWorker(
        VariantsIndexingOptions options,
        SmsIndexingHandler smsIndexingHandler,
        CnvsIndexingHandler cnvsIndexingHandler,
        SvsIndexingHandler svsIndexingHandler,
        ILogger<VariantsIndexingWorker> logger)
    {
        _options = options;
        _smsIndexingHandler = smsIndexingHandler;
        _cnvsIndexingHandler = cnvsIndexingHandler;
        _svsIndexingHandler = svsIndexingHandler;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Variants indexing worker started");

        stoppingToken.Register(() => _logger.LogInformation("Variants indexing worker stopped"));

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, stoppingToken);

        try
        {
            await _smsIndexingHandler.Prepare();
            await _cnvsIndexingHandler.Prepare();
            await _svsIndexingHandler.Prepare();
        }
        catch (Exception exception)
        {
            _logger.LogError("{error}", exception.GetShortMessage());
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _smsIndexingHandler.Handle(_options.SmBucketSize);
                await _cnvsIndexingHandler.Handle(_options.CnvBucketSize);
                await _svsIndexingHandler.Handle(_options.SvBucketSize);
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
