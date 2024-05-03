using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Variants.SSM;
using Unite.Genome.Feed.Web.Models.Variants.SSM.Binders;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.Variants;

[Route("api/dna/variants/ssms")]
[Authorize(Policy = Policies.Data.Writer)]
public class SsmsController : Controller
{
    private readonly VariantsSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public SsmsController(
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
        var submissionId = _submissionService.AddSsmSubmission(model);

        _submissionTaskService.CreateTask(SubmissionTaskType.SSM, submissionId);

        return Ok();
    }
}
