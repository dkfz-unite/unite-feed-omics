using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Dna.CnvProfile;
using Unite.Omics.Feed.Web.Models.Dna.CnvProfile.Validators;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Controllers.Dna;

[Route("api/dna/analysis/cnv-profile")]
[Authorize(Policy = Policies.Data.Writer)]
public class CnvProfileController(
    SubmissionTaskService submissionTaskService,
    ILogger<AnalysisDataController<CnvProfileModel>> logger,
    CnvProfileSubmissionRepository submissionRepository)
    : AnalysisDataController<CnvProfileModel>(submissionTaskService, submissionRepository, logger)
{
    protected override IValidator<CnvProfileModel> EntryModelValidator => new CnvProfileModelValidator();
    protected override string DataType => DataTypes.Omics.Dna.CnvProfile;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.WGS, AnalysisType.WES];
    protected override IReader<CnvProfileModel>[] Readers => 
    [
        new TsvReader<CnvProfileModel>()
    ];
    
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.DNA_CNVP;
}