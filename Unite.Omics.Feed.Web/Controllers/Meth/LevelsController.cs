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
    private readonly SubmissionTaskService _submissionTaskService;

    protected override string DataType => DataTypes.Omics.Meth.Level;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.MethArray, AnalysisType.WGBS, AnalysisType.RRBS];


    public LevelsController(
        MethSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }


    protected override AnalysisModel<EmptyModel> GetSubmission(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        return _submissionService.FindLevelSubmission(task.Target);
    }

    protected override long AddSubmission(AnalysisModel<EmptyModel> model, bool review)
    {
        var submissionId = _submissionService.AddLevelSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        return _submissionTaskService.CreateTask(SubmissionTaskType.METH_LVL, submissionId, taskStatus);
    }
}
