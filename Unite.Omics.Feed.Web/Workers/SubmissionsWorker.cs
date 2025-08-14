using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Handlers.Submission;

namespace Unite.Omics.Feed.Web.Workers;

public class SubmissionsWorker : BackgroundService
{
    private readonly BulkGeneExpSubmissionHandler _bulkGeneExpSubmissionHandler;
    private readonly CellGeneExpSubmissionHandler _cellGeneExpSubmissionHandler;
    private readonly SmsSubmissionHandler _smsSubmissionHandler;
    private readonly CnvsSubmissionHandler _cnvsSubmissionHandler;
    private readonly SvsSubmissionHandler _svsSubmissionHandler;
    private readonly MethSubmissionHandler _methSubmissionHandler;
    private readonly ILogger _logger;

    public SubmissionsWorker(
        BulkGeneExpSubmissionHandler bulkGeneExpSubmissionHandler,
        CellGeneExpSubmissionHandler cellGeneExpSubmissionHandler,
        SmsSubmissionHandler smsSubmissionHandler,
        CnvsSubmissionHandler cnvsSubmissionHandler,
        SvsSubmissionHandler svsSubmissionHandler,
        MethSubmissionHandler methSubmissionHandler,
        ILogger<SubmissionsWorker> logger)
    {
        _bulkGeneExpSubmissionHandler = bulkGeneExpSubmissionHandler;
        _cellGeneExpSubmissionHandler = cellGeneExpSubmissionHandler;
        _smsSubmissionHandler = smsSubmissionHandler;
        _cnvsSubmissionHandler = cnvsSubmissionHandler;
        _svsSubmissionHandler = svsSubmissionHandler;
        _methSubmissionHandler = methSubmissionHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Submissions worker started");

        stoppingToken.Register(() => _logger.LogInformation("Submissions worker stopped"));

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _cellGeneExpSubmissionHandler.Handle();
                _bulkGeneExpSubmissionHandler.Handle();
                _smsSubmissionHandler.Handle();
                _cnvsSubmissionHandler.Handle();
                _svsSubmissionHandler.Handle();
                _methSubmissionHandler.Handle();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Submissions processing failed");
            }
            finally
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
