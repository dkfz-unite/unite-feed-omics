using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Variants.SV;
using Unite.Genome.Feed.Web.Models.Variants.SV.Binders;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.Variants;

[Route("api/dna/variants/svs")]
[Authorize(Policy = Policies.Data.Writer)]
public class SvsController : Controller
{
    private readonly VariantsSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public SvsController(
        VariantsSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }


    [HttpPost("")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult Post([FromBody] SequencingDataModel<VariantModel> model)
    {
        return PostData(model);
    }

    [HttpPost("tsv")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult PostTsv([ModelBinder(typeof(TsvModelBinder))] SequencingDataModel<VariantModel> model)
    {
        return PostData(model);
    }


    private IActionResult PostData(SequencingDataModel<VariantModel> model)
    {
        var submissionId = _submissionService.AddSvSubmission(model);

        _submissionTaskService.CreateTask(SubmissionTaskType.SV, submissionId);

        return Ok();
    }
}
