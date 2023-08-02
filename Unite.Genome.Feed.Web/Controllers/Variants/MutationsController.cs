using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Genome.Feed.Web.Models.Variants;
using Unite.Genome.Feed.Web.Models.Variants.SSM;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.Variants;

[Route("api/dna/variants/ssms")]
[ApiController]
[Authorize]
public class MutationsController : Controller
{
    private readonly VariantsSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public MutationsController(
        VariantsSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }

    [HttpPost("")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult Post([FromBody] SequencingDataModel<VariantModel>[] models)
    {
        foreach (var model in models)
        {
            var submissionId = _submissionService.AddSsmSubmission(model);

            var submissionData = new SubmissionData(SubmissionType.Default);

            _submissionTaskService.CreateTask(SubmissionTaskType.SSM, submissionId, submissionData);
        }

        return Ok();
    }
}
