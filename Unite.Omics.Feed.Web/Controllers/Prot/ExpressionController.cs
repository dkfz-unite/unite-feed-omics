using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Prot;
using Unite.Omics.Feed.Web.Models.Prot.Validators;

namespace Unite.Omics.Feed.Web.Controllers.Prot;

[Route("api/prot/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController(
    SubmissionTaskService submissionTaskService,
    ILogger<ExpressionsController> logger,
    Submissions.Repositories.Prot.ExpressionSubmissionRepository submissionRepository)
    : AnalysisDataController<ExpressionModel>(submissionTaskService, submissionRepository, logger)
{
    protected override IValidator<ExpressionModel> EntryModelValidator => new ExpressionModelValidator();
    protected override string DataType => DataTypes.Omics.Proteomics.Expression;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.MS];
    protected override IReader<ExpressionModel>[] Readers =>
    [
        new TsvReader<ExpressionModel>()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.PROT_EXP;
}
