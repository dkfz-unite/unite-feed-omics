using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sm;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sm")]
[Authorize(Policy = Policies.Data.Writer)]
public class SmsController : SubmissionController<AnalysisModel<VariantModel>, AnalysisForm, VariantModel>
{
    protected override string DataType => DataTypes.Omics.Dna.Sm;
    protected override IValidator<AnalysisModel<VariantModel>> ModelValidator => new Models.Dna.Sm.Validators.AnalysisModelValidator();
    protected override IFormConverter<AnalysisModel<VariantModel>, AnalysisForm, VariantModel> FormConverter => new AnalysisFormConverter<VariantModel>();
    protected override IReader<VariantModel>[] EntryReaders =>
    [
        new TsvReader<VariantModel>(),
        new Models.Dna.Sm.Readers.Vcf.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_SM;


    public SmsController(SubmissionTaskService submissionTaskService,
        ILogger<SmsController> logger,
        SmSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
