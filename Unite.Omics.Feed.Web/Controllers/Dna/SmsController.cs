using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Dna.Sm;
using Unite.Omics.Feed.Web.Models.Dna.Sm.Validators;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sm")]
[Authorize(Policy = Policies.Data.Writer)]
public class SmsController : AnalysisDataController<VariantModel>
{
    private readonly DnaSubmissionService _submissionService;

    protected override IValidator<VariantModel> EntryModelValidator => new VariantModelValidator();
    protected override string DataType => DataTypes.Omics.Dna.Sm;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.WGS, AnalysisType.WES];
    protected override IReader<VariantModel>[] Readers =>
    [
        new Models.Dna.Sm.Readers.Tsv.Reader(),
        new Models.Dna.Sm.Readers.Vcf.Reader()
    ];


    public SmsController(
        DnaSubmissionService submissionService,
        SubmissionTaskService submissionTaskService,
        ILogger<SmsController> logger) : base(submissionTaskService, logger)
    {
        _submissionService = submissionService;
    }


    protected override AnalysisModel<VariantModel> FindSubmission(string id)
    {
        return _submissionService.FindSmSubmission(id);
    }

    protected override long AddSubmission(AnalysisModel<VariantModel> model, bool review)
    {
        var submissionId = _submissionService.AddSmSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        return _submissionTaskService.CreateTask(SubmissionTaskType.DNA_SM, submissionId, taskStatus);
    }
}
