using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class DnaSubmissionHandler : SampleSubmissionHandler
{
    private readonly DnaSubmissionService _submissionService;

    protected override SubmissionTaskType TaskType => SubmissionTaskType.DNA;


    public DnaSubmissionHandler(
        SampleWriter dataWriter,
        DnaSubmissionService submissionService,
        TasksProcessingService taskProcessingService,
        SampleIndexingTaskService indexingTaskService,
        ILogger<DnaSubmissionHandler> logger) : base(dataWriter, taskProcessingService, indexingTaskService, logger)
    {
        _submissionService = submissionService;
    }


    protected override Models.Base.SampleModel FindSubmission(string submissionId)
    {
        return _submissionService.FindSampleSubmission(submissionId);
    }

    protected override void DeleteSubmission(string submissionId)
    {
        _submissionService.DeleteSampleSubmission(submissionId);
    }
}
