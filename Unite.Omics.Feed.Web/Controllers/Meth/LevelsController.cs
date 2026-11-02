using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

namespace Unite.Omics.Feed.Web.Controllers.Meth;

// Not used yet: methylation data is currently submitted as samples (e.g. IDAT files) only.
[Route("api/meth/analysis/levels")]
[Authorize(Policy = Policies.Data.Writer)]
public class LevelsController : SubmissionController<AnalysisModel<EmptyModel>, AnalysisForm, EmptyModel>
{
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.METH_LVL;
    protected override string DataType => DataTypes.Omics.Methylation.Level;
    protected override IValidator<AnalysisModel<EmptyModel>> ModelValidator => new AnalysisResourcesModelValidator([AnalysisType.MethArray, AnalysisType.WGBS, AnalysisType.RRBS]);
    protected override IFormConverter<AnalysisModel<EmptyModel>, AnalysisForm, EmptyModel> FormConverter => new AnalysisFormConverter<EmptyModel>();


    public LevelsController(SubmissionTaskService submissionTaskService,
        ILogger<LevelsController> logger,
        LevelSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
