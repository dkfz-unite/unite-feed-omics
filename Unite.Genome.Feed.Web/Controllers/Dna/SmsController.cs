using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Dna.Sm;
using Unite.Genome.Feed.Web.Models.Dna.Sm.Binders;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sm")]
[Authorize(Policy = Policies.Data.Writer)]
public class SmsController : Controller
{
    private readonly DnaSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;
    private readonly IValidator<AnalysisModel<VariantModel>> _validator;

    public SmsController(
        DnaSubmissionService submissionService,
        SubmissionTaskService submissionTaskService,
        IValidator<AnalysisModel<VariantModel>> validator)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
        _validator = validator;
    }

    [HttpGet("{id}")]
    public IActionResult Get(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        var submission = _submissionService.FindSmSubmission(task.Target);

        return Ok(submission);
    }

    [HttpPost("")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult Post([FromBody] AnalysisModel<VariantModel> models, [FromQuery] bool review = true)
    {
        var submissionId = _submissionService.AddSmSubmission(models);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        var taskId = _submissionTaskService.CreateTask(SubmissionTaskType.DNA_SM, submissionId, taskStatus);

        return Ok(taskId);
    }

    [HttpPost("tsv")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult PostTsv([ModelBinder(typeof(AnalysisTsvModelBinder))] AnalysisModel<VariantModel> model, [FromQuery] bool review = true)
    {
        _validator.ValidateAndThrow(model);
        
        return Post(model, review);
    }
}
