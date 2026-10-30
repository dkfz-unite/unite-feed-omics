using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sv.Validators;
using Unite.Omics.Feed.Web.Models.Dna.Sv;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/sv")]
[Authorize(Policy = Policies.Data.Writer)]
public class SvsController : SubmissionController<AnalysisModel<VariantModel>, AnalysisForm, VariantModel>
{
    protected override string DataType => DataTypes.Omics.Dna.Sv;
    protected override IValidator<AnalysisModel<VariantModel>> ModelValidator => new AnalysisDataModelValidator<VariantModel>([AnalysisType.WGS, AnalysisType.WES], new VariantModelValidator());
    protected override IFormConverter<AnalysisModel<VariantModel>, AnalysisForm, VariantModel> FormConverter => new AnalysisFormConverter<VariantModel>();
    protected override IReader<VariantModel>[] EntryReaders =>
    [
        new TsvReader<VariantModel>(),
        new Models.Dna.Sv.Readers.DkfzSophia.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_SV;


    public SvsController(SubmissionTaskService submissionTaskService,
        ILogger<SvsController> logger,
        SvSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
