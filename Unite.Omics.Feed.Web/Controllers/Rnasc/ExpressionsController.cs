using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.RnaSc;

[Route("api/rnasc/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController : AnalysisController
{
    private readonly RnaScSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;
    protected override string DataType => DataTypes.Omics.Rnasc.Exp;


    public ExpressionsController(
        RnaScSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }

    public override IActionResult Get(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        var submission = _submissionService.FindExpSubmission(task.Target);

        return Ok(submission);
    }

    public override IActionResult PostJson([FromBody] AnalysisModel<EmptyModel> model, [FromQuery] bool review = true)
    {
        model.Resources?.ForEach(resource => resource.Type = DataType);

        var submissionId = _submissionService.AddExpSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        var taskId = _submissionTaskService.CreateTask(SubmissionTaskType.RNASC_EXP, submissionId, taskStatus);

        return Ok(taskId);
    }
}
