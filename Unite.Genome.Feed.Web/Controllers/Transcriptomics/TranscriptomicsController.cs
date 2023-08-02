using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Genome.Feed.Web.Models.Transcriptomics;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Controllers.Transcriptomics;

[Route("api/rna/expressions")]
[ApiController]
[Authorize]
public class TranscriptomicsController : Controller
{
    private readonly TranscriptomicsSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

	public TranscriptomicsController(
        TranscriptomicsSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
	{
		_submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }

    [HttpPost("")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult Post([FromBody] TranscriptomicsDataModel model)
	{
		var submissionId = _submissionService.AddSubmission(model);

        var submissionData = new SubmissionData(SubmissionType.Default);

        _submissionTaskService.CreateTask(SubmissionTaskType.TEX, submissionId, submissionData);

		return Ok();
	}
}
