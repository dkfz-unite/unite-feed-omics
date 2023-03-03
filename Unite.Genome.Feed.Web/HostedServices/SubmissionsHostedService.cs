using Unite.Genome.Feed.Web.Handlers.Submission;

namespace Unite.Genome.Feed.Web.HostedServices;

public class SubmissionsHostedService : BackgroundService
{
    private readonly TranscriptomicsSubmissionHandler _transcriptomicsSubmissionHandler;
    private readonly MutationsSubmissionHandler _mutationsSubmissionHandler;
    private readonly CopyNumberVariantsSubmissionHandler _copyNumberVariantsSubmissionHandler;
    private readonly StructuralVariantsSubmissionHandler _structuralVariantsSubmissionHandler;
    private readonly ILogger _logger;

    public SubmissionsHostedService(
        TranscriptomicsSubmissionHandler transcriptomicsSubmissionHandler,
        MutationsSubmissionHandler mutationsSubmissionHandler,
        CopyNumberVariantsSubmissionHandler copyNumberVariantsSubmissionHandler,
        StructuralVariantsSubmissionHandler structuralVariantsSubmissionHandler,
        ILogger<SubmissionsHostedService> logger)
    {
        _transcriptomicsSubmissionHandler = transcriptomicsSubmissionHandler;
        _mutationsSubmissionHandler = mutationsSubmissionHandler;
        _copyNumberVariantsSubmissionHandler = copyNumberVariantsSubmissionHandler;
        _structuralVariantsSubmissionHandler = structuralVariantsSubmissionHandler;
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
                _transcriptomicsSubmissionHandler.Handle();
                _mutationsSubmissionHandler.Handle();
                _copyNumberVariantsSubmissionHandler.Handle();
                _structuralVariantsSubmissionHandler.Handle();
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

