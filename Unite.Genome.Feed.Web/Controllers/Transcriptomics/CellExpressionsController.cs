using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Transcriptomics;
using Unite.Genome.Feed.Web.Models.Transcriptomics.Binders;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.Transcriptomics;

[Route("api/rna/exp/cell")]
[Authorize(Policy = Policies.Data.Writer)]
public class CellExpressionsController : Controller
{
    private readonly TranscriptomicsSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public CellExpressionsController(
        TranscriptomicsSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }


    [HttpPost("")]
    [Consumes("application/json")]
    public IActionResult Post([FromBody] SequencingDataModel<CellExpressionModel> model)
    {
        return PostData(model);
    }

    [HttpPost("tsv")]
    [Consumes("text/tab-separated-values")]
    public IActionResult PostTsv([ModelBinder(typeof(CellExpressionTsvModelsBinder))] SequencingDataModel<CellExpressionModel> model)
    {
        return PostData(model);
    }


    private IActionResult PostData(SequencingDataModel<CellExpressionModel> model)
    {
        var submissionId = _submissionService.AddCellSubmission(model);

        _submissionTaskService.CreateTask(SubmissionTaskType.CGE, submissionId);

        return Ok();
    }
}
