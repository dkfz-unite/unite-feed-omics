using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class MethSubmissionHandler : SampleSubmissionHandler
{
    private readonly SampleSubmissionRepository _submissionRepository;

    protected override SubmissionTaskType TaskType => SubmissionTaskType.METH;


    public MethSubmissionHandler(
        SampleWriter dataWriter,
        TasksProcessingService taskProcessingService,
        SampleIndexingTaskService indexingTaskService,
        SampleSubmissionRepository submissionRepository,
        ILogger<MethSubmissionHandler> logger) : base(dataWriter, taskProcessingService, indexingTaskService, logger)
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
