using Unite.Omics.Feed.Web.Handlers.Submission;

namespace Unite.Omics.Feed.Web.Workers;

public class SubmissionsWorker : BackgroundService
{
    private readonly RnaSubmissionHandler _rnaSubmissionHandler;
    private readonly RnaExpSubmissionHandler _rnaExpSubmissionHandler;
    private readonly RnascSubmissionHandler _rnascSubmissionHandler;
    private readonly RnascExpSubmissionHandler _rnascExpSubmissionHandler;
    private readonly DnaSubmissionHandler _dnaSubmissionHandler;
    private readonly DnaSmSubmissionHandler _dnaSmSubmissionHandler;
    private readonly DnaCnvSubmissionHandler _dnaCnvSubmissionHandler;
    private readonly DnaSvSubmissionHandler _dnaSvSubmissionHandler;
    private readonly MethSubmissionHandler _methSubmissionHandler;
    private readonly MethLvlSubmissionHandler _methLvlSubmissionHandler;
    private readonly ILogger _logger;


    public SubmissionsWorker(
        RnaSubmissionHandler rnaSubmissionHandler,
        RnaExpSubmissionHandler rnaExpSubmissionHandler,
        RnascSubmissionHandler rnascSubmissionHandler,
        RnascExpSubmissionHandler rnascExpSubmissionHandler,
        DnaSubmissionHandler dnaSubmissionHandler,
        DnaSmSubmissionHandler dnaSmSubmissionHandler,
        DnaCnvSubmissionHandler dnaCnvSubmissionHandler,
        DnaSvSubmissionHandler dnaSvSubmissionHandler,
        MethSubmissionHandler methSubmissionHandler,
        MethLvlSubmissionHandler methLvlSubmissionHandler,
        ILogger<SubmissionsWorker> logger)
    {
        _rnaSubmissionHandler = rnaSubmissionHandler;
        _rnaExpSubmissionHandler = rnaExpSubmissionHandler;
        _rnascSubmissionHandler = rnascSubmissionHandler;
        _rnascExpSubmissionHandler = rnascExpSubmissionHandler;
        _dnaSubmissionHandler = dnaSubmissionHandler;
        _dnaSmSubmissionHandler = dnaSmSubmissionHandler;
        _dnaCnvSubmissionHandler = dnaCnvSubmissionHandler;
        _dnaSvSubmissionHandler = dnaSvSubmissionHandler;
        _methSubmissionHandler = methSubmissionHandler;
        _methLvlSubmissionHandler = methLvlSubmissionHandler;
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
                _rnaSubmissionHandler.Handle();
                _rnaExpSubmissionHandler.Handle();
                _rnascSubmissionHandler.Handle();
                _rnascExpSubmissionHandler.Handle();
                _dnaSubmissionHandler.Handle();
                _dnaSmSubmissionHandler.Handle();
                _dnaCnvSubmissionHandler.Handle();
                _dnaSvSubmissionHandler.Handle();
                _methSubmissionHandler.Handle();
                _methLvlSubmissionHandler.Handle();
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
