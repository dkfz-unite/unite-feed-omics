using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

namespace Unite.Omics.Feed.Web.Controllers.RnaSc;

[Route("api/rnasc/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController : SubmissionController<AnalysisModel<EmptyModel>, AnalysisForm, EmptyModel>
{
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.RNASC_EXP;
    protected override string DataType => DataTypes.Omics.Rnasc.Expression;
    protected override IValidator<AnalysisModel<EmptyModel>> ModelValidator => new Models.RnaSc.Validators.AnalysisModelValidator();
    protected override IFormConverter<AnalysisModel<EmptyModel>, AnalysisForm, EmptyModel> FormConverter => new AnalysisFormConverter<EmptyModel>();


    public ExpressionsController(SubmissionTaskService submissionTaskService,
        ILogger<ExpressionsController> logger,
        ExpressionSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
