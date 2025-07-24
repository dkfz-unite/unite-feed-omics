using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Dna.Sm;
using Unite.Omics.Feed.Web.Models.Dna.Sm.Binders;
using Unite.Omics.Feed.Web.Models.Dna.Sm.Validators;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sm")]
[Authorize(Policy = Policies.Data.Writer)]
public class SmsController : AnalysisController<VariantModel>
{
    private readonly DnaSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    protected override IValidator<VariantModel> EntryModelValidator => new VariantModelValidator();
    protected override IValidator<ResourceModel> ResourceModelValidator => new ResourceModelValidator();
    protected override IReader<VariantModel>[] Readers =>
    [
        new Models.Dna.Sm.Readers.Tsv.Reader(),
        new Models.Dna.Sm.Readers.Vcf.Reader()
    ]; 


    public SmsController(
        DnaSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }


    [HttpGet("{id}")]
    public override IActionResult Get(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        var submission = _submissionService.FindSmSubmission(task.Target);

        return Ok(submission);
    }

    [HttpPost("")]
    [RequestSizeLimit(100_000_000)]
    public override IActionResult Post([FromBody] AnalysisModel<VariantModel> model, [FromQuery] bool review = true)
    {
        var submissionId = _submissionService.AddSmSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        var taskId = _submissionTaskService.CreateTask(SubmissionTaskType.DNA_SM, submissionId, taskStatus);

        return Ok(taskId);
    }

    [HttpPost("tsv")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult PostTsv([ModelBinder(typeof(AnalysisTsvModelBinder))] AnalysisModel<VariantModel> model, [FromQuery] bool review = true)
    {
        return TryValidateModel(model) ? Post(model, review) : BadRequest(ModelState);
    }
}
