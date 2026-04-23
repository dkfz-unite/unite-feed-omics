using Unite.Omics.Feed.Web.Handlers.Submission;

namespace Unite.Omics.Feed.Web.Workers;

public class SubmissionsWorker : Worker
{
    public SubmissionsWorker(
        RnaSubmissionHandler rnaSubmissionHandler,
        RnaExpSubmissionHandler rnaExpSubmissionHandler,
        ProtExpSubmissionHandler protExpSubmissionHandler,
        RnascSubmissionHandler rnascSubmissionHandler,
        RnascExpSubmissionHandler rnascExpSubmissionHandler,
        DnaSubmissionHandler dnaSubmissionHandler,
        DnaSmSubmissionHandler dnaSmSubmissionHandler,
        DnaCnvSubmissionHandler dnaCnvSubmissionHandler,
        DnaSvSubmissionHandler dnaSvSubmissionHandler,
        MethSubmissionHandler methSubmissionHandler,
        MethLvlSubmissionHandler methLvlSubmissionHandler,
        CnvProfileSubmissionHandler cnvProfileSubmissionHandler,
        ILogger<SubmissionsWorker> logger) : base(logger)
    {
        // Ordered by priority from highest to lowest.
        _handlers = [
            rnaSubmissionHandler,
            rnaExpSubmissionHandler,
            protExpSubmissionHandler,
            rnascSubmissionHandler,
            rnascExpSubmissionHandler,
            dnaSubmissionHandler,
            dnaSmSubmissionHandler,
            dnaCnvSubmissionHandler,
            dnaSvSubmissionHandler,
            methSubmissionHandler,
            methLvlSubmissionHandler,
            cnvProfileSubmissionHandler
        ];
    }
}
