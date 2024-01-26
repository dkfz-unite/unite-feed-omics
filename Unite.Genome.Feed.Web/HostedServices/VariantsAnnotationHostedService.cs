using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers.Annotation;

namespace Unite.Genome.Feed.Web.HostedServices;

public class VariantsAnnotationHostedService : BackgroundService
{
    private readonly VariantsAnnotationOptions _options;
    private readonly SsmsAnnotationHandler _ssmsAnnotationHandler;
    private readonly CnvsAnnotationHandler _cnvsAnnotationHandler;
    private readonly SvsAnnotationHandler _svsAnnotationHandler;
    private readonly ILogger _logger;


    public VariantsAnnotationHostedService(
        VariantsAnnotationOptions options,
        SsmsAnnotationHandler ssmsAnnotationHandler,
        CnvsAnnotationHandler cnvsAnnotationHandler,
        SvsAnnotationHandler svsAnnotationHandler,
        ILogger<VariantsAnnotationHostedService> logger)
    {
        _options = options;
        _ssmsAnnotationHandler = ssmsAnnotationHandler;
        _cnvsAnnotationHandler = cnvsAnnotationHandler;
        _svsAnnotationHandler = svsAnnotationHandler;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Variants annotation service started");

        stoppingToken.Register(() => _logger.LogInformation("Variants annotation service stopped"));

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, stoppingToken);

        try
        {
            _ssmsAnnotationHandler.Prepare();
            _cnvsAnnotationHandler.Prepare();
            _svsAnnotationHandler.Prepare();
        }
        catch (Exception exception)
        {
            _logger.LogError("{error}", exception.GetShortMessage());
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _ssmsAnnotationHandler.Handle(_options.SsmBucketSize);
                _cnvsAnnotationHandler.Handle(_options.CnvBucketSize);
                _svsAnnotationHandler.Handle(_options.SvBucketSize);
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
