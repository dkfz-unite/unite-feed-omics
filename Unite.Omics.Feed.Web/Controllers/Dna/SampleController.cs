using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/sample")]
public class SampleController : Controllers.SampleController
{
    private readonly DnaSubmissionService _submissionService;
    protected override string DataType => DataTypes.Omics.Dna.Sample;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.WGS, AnalysisType.WES];


    public SampleController(DnaSubmissionService submissionService, SubmissionTaskService submissionTaskService, ILogger<SampleController> logger) : base(submissionTaskService, logger)
    {
        _submissionService = submissionService;
    }


    protected override SampleModel GetSubmission(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        return _submissionService.FindSampleSubmission(task.Target);
    }

    protected override long AddSubmission(SampleModel model, bool review)
    {
        var submissionId = _submissionService.AddSampleSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        return _submissionTaskService.CreateTask(SubmissionTaskType.DNA, submissionId, taskStatus);
    }
}
