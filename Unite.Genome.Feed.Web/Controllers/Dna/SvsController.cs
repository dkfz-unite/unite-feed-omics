using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Dna.Sv;
using Unite.Genome.Feed.Web.Models.Dna.Sv.Binders;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sv")]
[Authorize(Policy = Policies.Data.Writer)]
public class SvsController : Controller
{
    private readonly DnaSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public SvsController(
        DnaSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }


    [HttpPost("")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult Post([FromBody] AnalysisModel<VariantModel> model)
    {
        var submissionId = _submissionService.AddSvSubmission(model);

        _submissionTaskService.CreateTask(SubmissionTaskType.DNA_SV, submissionId);

        return Ok();
    }

    [HttpPost("tsv")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult PostTsv([ModelBinder(typeof(AnalysisTsvModelBinder))] AnalysisModel<VariantModel> model)
    {
        return Post(model);
    }
}
