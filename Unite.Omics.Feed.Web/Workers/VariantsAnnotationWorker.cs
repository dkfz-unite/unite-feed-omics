using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers.Annotation;

namespace Unite.Omics.Feed.Web.Workers;

public class VariantsAnnotationWorker : BackgroundService
{
    private readonly VariantsAnnotationOptions _options;
    private readonly SmsAnnotationHandler _smsAnnotationHandler;
    private readonly CnvsAnnotationHandler _cnvsAnnotationHandler;
    private readonly SvsAnnotationHandler _svsAnnotationHandler;
    private readonly ILogger _logger;


    public VariantsAnnotationWorker(
        VariantsAnnotationOptions options,
        SmsAnnotationHandler smsAnnotationHandler,
        CnvsAnnotationHandler cnvsAnnotationHandler,
        SvsAnnotationHandler svsAnnotationHandler,
        ILogger<VariantsAnnotationWorker> logger)
    {
        _options = options;
        _smsAnnotationHandler = smsAnnotationHandler;
        _cnvsAnnotationHandler = cnvsAnnotationHandler;
        _svsAnnotationHandler = svsAnnotationHandler;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Variants annotation worker started");

        stoppingToken.Register(() => _logger.LogInformation("Variants annotation worker stopped"));

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, stoppingToken);

        try
        {
            _smsAnnotationHandler.Prepare();
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
                _smsAnnotationHandler.Handle(_options.SmBucketSize);
                _cnvsAnnotationHandler.Handle(_options.CnvBucketSize);
                _svsAnnotationHandler.Handle(_options.SvBucketSize);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Variants annotation processing failed");
            }
            finally
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
