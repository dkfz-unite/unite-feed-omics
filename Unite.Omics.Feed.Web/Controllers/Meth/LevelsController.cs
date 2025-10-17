using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.Meth;

[Route("api/meth/analysis/levels")]
[Authorize(Policy = Policies.Data.Writer)]
public class LevelsController : AnalysisController
{
    private readonly MethSubmissionService _submissionService;

    protected override string DataType => DataTypes.Omics.Meth.Level;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.MethArray, AnalysisType.WGBS, AnalysisType.RRBS];


    public LevelsController(
        MethSubmissionService submissionService,
        SubmissionTaskService submissionTaskService,
        ILogger<LevelsController> logger) : base(submissionTaskService, logger)
    {
        _submissionService = submissionService;
    }


    protected override AnalysisModel<EmptyModel> FindSubmission(string id)
    {
        return _submissionService.FindLevelSubmission(id);
    }

    protected override long AddSubmission(AnalysisModel<EmptyModel> model, bool review)
    {
        var submissionId = _submissionService.AddLevelSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        return _submissionTaskService.CreateTask(SubmissionTaskType.METH_LVL, submissionId, taskStatus);
    }
}
