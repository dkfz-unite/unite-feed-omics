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

[Route("api/rna/exp/cell")]
[Authorize(Policy = Policies.Data.Writer)]
public class CellExpressionsController : Controller
{
    private readonly RnaSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public CellExpressionsController(
        RnaSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }


    [HttpPost("")]
    [Consumes("application/json")]
    public IActionResult Post([FromBody] SeqDataModel<CellExpressionModel> model)
    {
        return PostData(model);
    }

    [HttpPost("tsv")]
    [Consumes("text/tab-separated-values")]
    public IActionResult PostTsv([ModelBinder(typeof(CellExpressionTsvModelsBinder))] SeqDataModel<CellExpressionModel> model)
    {
        return PostData(model);
    }


    private IActionResult PostData(SeqDataModel<CellExpressionModel> model)
    {
        var submissionId = _submissionService.AddCellSubmission(model);

        _submissionTaskService.CreateTask(SubmissionTaskType.CGE, submissionId);

        return Ok();
    }
}
