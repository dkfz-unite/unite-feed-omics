using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class RnascSubmissionHandler : SampleSubmissionHandler
{
    private readonly SampleSubmissionRepository _submissionRepository;

    protected override SubmissionTaskType TaskType => SubmissionTaskType.RNASC;


    public RnascSubmissionHandler(
        HandlerPriority priority,
        SampleWriter dataWriter,
        TasksProcessingService taskProcessingService,
        SampleIndexingTaskService indexingTaskService,
        SampleSubmissionRepository submissionRepository,
        ILogger<RnascSubmissionHandler> logger) : base(priority, dataWriter, taskProcessingService, indexingTaskService, logger)
    {
        _submissionRepository = submissionRepository;
    }


    protected override SampleModel FindSubmission(string submissionId)
    {
        return _submissionRepository.FindDocument(submissionId);
    }

    protected override void DeleteSubmission(string submissionId)
    {
        _submissionRepository.Delete(submissionId);
    }
}
