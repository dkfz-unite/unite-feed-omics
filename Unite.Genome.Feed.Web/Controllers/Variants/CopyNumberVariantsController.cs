using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Genome.Feed.Web.Models.Variants;
using Unite.Genome.Feed.Web.Models.Variants.CNV;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.Variants;

[Route("api/dna/variants/cnvs")]
[ApiController]
[Authorize]
public class CopyNumberVariantsController : Controller
{
    private readonly VariantsSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    public CopyNumberVariantsController(
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
            var submissionId = _submissionService.AddCnvSubmission(model);

            var submissionData = new SubmissionData(SubmissionType.Default);

            _submissionTaskService.CreateTask(SubmissionTaskType.CNV, submissionId, submissionData);
        }

        return Ok();
    }

    [HttpPost("aceseq")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult Post([FromBody] SequencingDataModel<VariantAceSeqModel>[] models)
    {
        foreach (var model in models)
        {
            var submissionId = _submissionService.AddCnvAceSeqSubmission(model);

            var submissionData = new SubmissionData(SubmissionType.AceSeq);

            _submissionTaskService.CreateTask(SubmissionTaskType.CNV, submissionId, submissionData);
        }

        return Ok();
    }
}
