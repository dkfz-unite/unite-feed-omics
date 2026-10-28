using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

namespace Unite.Omics.Feed.Web.Controllers.RnaSc;

[Route("api/rnasc/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController : SubmissionController<AnalysisModel<EmptyModel>, AnalysisForm, EmptyModel>
{
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.RNASC_EXP;
    protected override string DataType => DataTypes.Omics.Rnasc.Expression;
    // Resource files can arrive independently; a complete matrix bundle is not required in one request.
    protected override IValidator<AnalysisModel<EmptyModel>> ModelValidator => new AnalysisResourcesModelValidator([AnalysisType.RNASeqSc, AnalysisType.RNASeqSn]);
    protected override IFormConverter<AnalysisModel<EmptyModel>, AnalysisForm, EmptyModel> FormConverter => new AnalysisFormConverter<EmptyModel>();


    public ExpressionsController(SubmissionTaskService submissionTaskService,
        ILogger<ExpressionsController> logger,
        ExpressionSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
