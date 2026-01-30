using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class DnaSubmissionHandler : SampleSubmissionHandler
{
    private readonly SampleSubmissionRepository _submissionRepository;

    protected override SubmissionTaskType TaskType => SubmissionTaskType.DNA;


    public DnaSubmissionHandler(
        SampleWriter dataWriter,
        TasksProcessingService taskProcessingService,
        SampleIndexingTaskService indexingTaskService,
        SampleSubmissionRepository submissionRepository,
        ILogger<DnaSubmissionHandler> logger) : base(dataWriter, taskProcessingService, indexingTaskService, logger)
    {
        _submissionRepository = submissionRepository;
    }


    protected override Models.Base.SampleModel FindSubmission(string submissionId)
    {
        return _submissionRepository.FindDocument<Models.Base.SampleModel>(submissionId);
    }

    protected override void DeleteSubmission(string submissionId)
    {
        _submissionRepository.Delete<Models.Base.SampleModel>(submissionId);
    }
}
