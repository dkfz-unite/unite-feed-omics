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
using Unite.Omics.Feed.Web.Models.Prot.Validators;
using Unite.Omics.Feed.Web.Models.Prot;

namespace Unite.Omics.Feed.Web.Controllers.Prot;

[Route("api/prot/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController : SubmissionController<AnalysisModel<ExpressionModel>, AnalysisForm, ExpressionModel>
{
    protected override string DataType => DataTypes.Omics.Proteomics.Expression;
    protected override IValidator<AnalysisModel<ExpressionModel>> ModelValidator => new AnalysisDataModelValidator<ExpressionModel>([AnalysisType.MS], new ExpressionModelValidator());
    protected override IFormConverter<AnalysisModel<ExpressionModel>, AnalysisForm, ExpressionModel> FormConverter => new AnalysisFormConverter<ExpressionModel>();
    protected override IReader<ExpressionModel>[] EntryReaders =>
    [
        new TsvReader<ExpressionModel>(),
        new Models.Prot.Readers.DiaNN.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.PROT_EXP;


    public ExpressionsController(SubmissionTaskService submissionTaskService,
        ILogger<ExpressionsController> logger,
        Submissions.Repositories.Prot.ExpressionSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
