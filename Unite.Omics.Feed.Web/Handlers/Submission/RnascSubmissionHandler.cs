using System;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class RnascSubmissionHandler : SampleSubmissionHandler
{
    private readonly RnascSubmissionService _submissionService;

    protected override SubmissionTaskType TaskType => SubmissionTaskType.RNASC;


    public RnascSubmissionHandler(
        SampleWriter dataWriter,
        RnascSubmissionService submissionService,
        TasksProcessingService taskProcessingService,
        SampleIndexingTaskService indexingTaskService,
        ILogger<RnascSubmissionHandler> logger) : base(dataWriter, taskProcessingService, indexingTaskService, logger)
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
