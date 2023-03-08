using Microsoft.AspNetCore.Mvc;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Genome.Feed.Web.Models.Variants;
using Unite.Genome.Feed.Web.Models.Variants.SV;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.Variants;

[Route("api/dna/variants/svs")]
[ApiController]
public class StructuralVariantsController : Controller
{
    private readonly VariantsSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public StructuralVariantsController(
        VariantsSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }

    [HttpPost("")]
    public IActionResult Post([FromBody] SequencingDataModel<VariantModel>[] models)
    {
        foreach (var model in models)
        {
            var submissionId = _submissionService.AddSvSubmission(model);

            var submissionData = new SubmissionData(SubmissionType.Default);

            _submissionTaskService.CreateTask(SubmissionTaskType.SV, submissionId, submissionData);
        }

        return Ok();
    }
}
