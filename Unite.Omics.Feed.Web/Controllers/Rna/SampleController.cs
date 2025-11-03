using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.Rna;

[Route("api/rna/sample")]
public class SampleController : Controllers.SampleController
{
    private readonly RnaSubmissionService _submissionService;
    protected override string DataType => DataTypes.Omics.Rna.Sample;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.RNASeq];


    public SampleController(RnaSubmissionService submissionService, SubmissionTaskService submissionTaskService, ILogger<SampleController> logger) : base(submissionTaskService, logger)
    {
        _submissionService = submissionService;
    }


    protected override SampleModel FindSubmission(string id)
    {
        return _submissionService.FindSampleSubmission(id);
    }
    
    protected override long AddSubmission(SampleModel model, bool review)
    {
        var submissionId = _submissionService.AddSampleSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        return _submissionTaskService.CreateTask(SubmissionTaskType.RNA, submissionId, taskStatus);
    }
}
