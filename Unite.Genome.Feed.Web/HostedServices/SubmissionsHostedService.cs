using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Web.Handlers.Submission;

namespace Unite.Genome.Feed.Web.HostedServices;

public class SubmissionsHostedService : BackgroundService
{
    private readonly BulkTranscriptomicsSubmissionHandler _bulkTranscriptomicsSubmissionHandler;
    private readonly CellTranscriptomicsSubmissionHandler _cellTranscriptomicsSubmissionHandler;
    private readonly SsmsSubmissionHandler _ssmsSubmissionHandler;
    private readonly CnvsSubmissionHandler _cnvsSubmissionHandler;
    private readonly SvsSubmissionHandler _svsSubmissionHandler;
    private readonly ILogger _logger;

    public SubmissionsHostedService(
        BulkTranscriptomicsSubmissionHandler bulkTranscriptomicsSubmissionHandler,
        CellTranscriptomicsSubmissionHandler cellTranscriptomicsSubmissionHandler,
        SsmsSubmissionHandler ssmsSubmissionHandler,
        CnvsSubmissionHandler cnvsSubmissionHandler,
        SvsSubmissionHandler svsSubmissionHandler,
        ILogger<SubmissionsHostedService> logger)
    {
        _bulkTranscriptomicsSubmissionHandler = bulkTranscriptomicsSubmissionHandler;
        _cellTranscriptomicsSubmissionHandler = cellTranscriptomicsSubmissionHandler;
        _ssmsSubmissionHandler = ssmsSubmissionHandler;
        _cnvsSubmissionHandler = cnvsSubmissionHandler;
        _svsSubmissionHandler = svsSubmissionHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Submissions service started");

        stoppingToken.Register(() => _logger.LogInformation("Submissions service stopped"));

        // Delay 5 seconds to let the web api start working
        await Task.Delay(5000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _cellTranscriptomicsSubmissionHandler.Handle();
                _bulkTranscriptomicsSubmissionHandler.Handle();
                _ssmsSubmissionHandler.Handle();
                _cnvsSubmissionHandler.Handle();
                _svsSubmissionHandler.Handle();
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
