using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Rna;
using Unite.Genome.Feed.Web.Models.Rna.Binders;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.Rna;

[Route("api/rna/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController : Controller
{
    private readonly RnaSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;
    private readonly IValidator<AnalysisModel<ExpressionModel>> _validator;

	public ExpressionsController(
        RnaSubmissionService submissionService,
        SubmissionTaskService submissionTaskService,
        IValidator<AnalysisModel<ExpressionModel>> validator)
	{
		_submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
        _validator = validator;
    }

    [HttpGet("{id}")]
    public IActionResult Get(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        var submission = _submissionService.FindExpSubmission(task.Target);

        return Ok(submission);
    }

    [HttpPost("")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult Post([FromBody] AnalysisModel<ExpressionModel> model, [FromQuery] bool review = true)
    {
        var submissionId = _submissionService.AddExpSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        var taskId = _submissionTaskService.CreateTask(SubmissionTaskType.RNA_EXP, submissionId, taskStatus);

        return Ok(taskId);
    }

    [HttpPost("tsv")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult PostTsv([ModelBinder(typeof(AnalysisTsvModelsBinder))] AnalysisModel<ExpressionModel> model, [FromQuery] bool review = true)
    {
        _validator.ValidateAndThrow(model);
        
        return Post(model, review);
    }
}
