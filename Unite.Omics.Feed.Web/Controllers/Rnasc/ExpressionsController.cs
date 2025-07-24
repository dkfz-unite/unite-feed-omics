using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.RnaSc;
using Unite.Omics.Feed.Web.Models.RnaSc.Binders;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.RnaSc;

[Route("api/rnasc/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController : AnalysisController<ExpressionModel>
{
    private readonly RnaScSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    protected override IValidator<ExpressionModel> EntryModelValidator => null;
    protected override IValidator<ResourceModel> ResourceModelValidator => new ResourceModelValidator();
    protected override IReader<ExpressionModel>[] Readers => null;


    public ExpressionsController(
        RnaScSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }


    [HttpGet("{id}")]
    public override IActionResult Get(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        var submission = _submissionService.FindExpSubmission(task.Target);

        return Ok(submission);
    }

    [HttpPost("")]
    [RequestSizeLimit(100_000_000)]
    public override IActionResult Post([FromBody] AnalysisModel<ExpressionModel> model, [FromQuery] bool review = true)
    {
        var submissionId = _submissionService.AddExpSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        var taskId = _submissionTaskService.CreateTask(SubmissionTaskType.RNASC_EXP, submissionId, taskStatus);

        return Ok(taskId);
    }

    [HttpPost("tsv")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult PostTsv([ModelBinder(typeof(AnalysisTsvModelsBinder))] AnalysisModel<ResourceModel> model, [FromQuery] bool review = true)
    {
        var analysisModel = new AnalysisModel<ExpressionModel>
        {
            TargetSample = model.TargetSample,
            MatchedSample = model.MatchedSample,
            Resources = model.Entries
        };

        return TryValidateModel(analysisModel) ? Post(analysisModel, review) : BadRequest(ModelState);
    }
}
