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
using Unite.Omics.Feed.Web.Models.Dna.Cnv.Validators;
using Unite.Omics.Feed.Web.Models.Dna.Cnv;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/cnvp")]
[Authorize(Policy = Policies.Data.Writer)]
public class CnvpController : SubmissionController<AnalysisModel<ProfileModel>, AnalysisForm, ProfileModel>
{
    protected override string DataType => DataTypes.Omics.Dna.CnvProfile;
    protected override IValidator<AnalysisModel<ProfileModel>> ModelValidator => new AnalysisDataModelValidator<ProfileModel>([AnalysisType.WGS, AnalysisType.WES, AnalysisType.MethArray], new ProfileModelValidator());
    protected override IFormConverter<AnalysisModel<ProfileModel>, AnalysisForm, ProfileModel> FormConverter => new AnalysisFormConverter<ProfileModel>();
    protected override IReader<ProfileModel>[] EntryReaders =>
    [
        new TsvReader<ProfileModel>()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_CNVP;


    public CnvpController(SubmissionTaskService submissionTaskService,
        ILogger<CnvpController> logger,
        CnvpSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
