using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Dna.Meth;
using Unite.Genome.Feed.Web.Models.Dna.Meth.Binders;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.RnaSc;

[Route("api/dna/analysis/meth")]
[Authorize(Policy = Policies.Data.Writer)]
public class MethController : Controller
{
    private readonly DnaSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public MethController(
        DnaSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }

    [HttpGet("{id}")]
    public IActionResult Get(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        var submission = _submissionService.FindMethSubmission(task.Target);

        return Ok(submission);
    }

    [HttpPost("")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult Post([FromBody] AnalysisModel<ExpressionModel> model, [FromQuery] bool review = true)
    {
        var submissionId = _submissionService.AddMethSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        var taskId = _submissionTaskService.CreateTask(SubmissionTaskType.DNA_METH, submissionId, taskStatus);

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

        return Post(analysisModel, review);
    }
}
