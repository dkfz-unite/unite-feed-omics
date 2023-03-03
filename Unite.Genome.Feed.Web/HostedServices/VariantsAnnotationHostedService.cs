using Unite.Genome.Feed.Web.Configuration.Options;
using Unite.Genome.Feed.Web.Handlers.Annotation;

namespace Unite.Genome.Feed.Web.HostedServices;

public class VariantsAnnotationHostedService : BackgroundService
{
    private readonly VariantsAnnotationOptions _options;
    private readonly MutationsAnnotationHandler _mutationsAnnotationHandler;
    private readonly CopyNumberVariantsAnnotationHandler _copyNumberVariantsAnnotationHandler;
    private readonly StructuralVariantsAnnotationHandler _structuralVariantsAnnotationHandler;
    private readonly ILogger _logger;


    public VariantsAnnotationHostedService(
        VariantsAnnotationOptions options,
        MutationsAnnotationHandler mutationsAnnotationHandler,
        CopyNumberVariantsAnnotationHandler copyNumberVariantsAnnotationHandler,
        StructuralVariantsAnnotationHandler structuralVariantsAnnotationHandler,
        ILogger<VariantsAnnotationHostedService> logger)
    {
        _options = options;
        _mutationsAnnotationHandler = mutationsAnnotationHandler;
        _copyNumberVariantsAnnotationHandler = copyNumberVariantsAnnotationHandler;
        _structuralVariantsAnnotationHandler = structuralVariantsAnnotationHandler;
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
            _mutationsAnnotationHandler.Prepare();
            _copyNumberVariantsAnnotationHandler.Prepare();
            _structuralVariantsAnnotationHandler.Prepare();
        }
        catch (Exception exception)
        {
            LogError(exception);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _mutationsAnnotationHandler.Handle(_options.SsmBucketSize);
                _copyNumberVariantsAnnotationHandler.Handle(_options.CnvBucketSize);
                _structuralVariantsAnnotationHandler.Handle(_options.SvBucketSize);
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
