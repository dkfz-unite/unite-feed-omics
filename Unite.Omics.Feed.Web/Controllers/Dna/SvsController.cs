using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Dna.Sv;
using Unite.Omics.Feed.Web.Models.Dna.Sv.Validators;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sv")]
[Authorize(Policy = Policies.Data.Writer)]
public class SvsController : AnalysisDataController<VariantModel>
{
    private readonly DnaSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    protected override IValidator<VariantModel> EntryModelValidator => new VariantModelValidator();
    protected override IValidator<ResourceModel> ResourceModelValidator => new ResourceModelValidator();
    protected override string DataType => DataTypes.Omics.Dna.Sv;
    protected override IReader<VariantModel>[] Readers =>
    [
        new Models.Dna.Sv.Readers.Tsv.Reader(),
        new Models.Dna.Sv.Readers.DkfzSophia.Reader()
    ]; 


    public SvsController(
        DnaSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }


    public override IActionResult Get(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        var submission = _submissionService.FindSvSubmission(task.Target);

        return Ok(submission);
    }

    public override IActionResult PostJson([FromBody] AnalysisModel<VariantModel> model, [FromQuery] bool review = true)
    {
        model.Resources?.ForEach(resource => resource.Type = DataType);

        var submissionId = _submissionService.AddSvSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        var taskId = _submissionTaskService.CreateTask(SubmissionTaskType.DNA_SV, submissionId, taskStatus);

        return Ok(taskId);
    }
}
