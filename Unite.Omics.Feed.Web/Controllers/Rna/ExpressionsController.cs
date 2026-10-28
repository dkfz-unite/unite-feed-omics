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
using Unite.Omics.Feed.Web.Models.Rna.Validators;
using Unite.Omics.Feed.Web.Models.Rna;

namespace Unite.Omics.Feed.Web.Controllers.Rna;

[Route("api/rna/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController : SubmissionController<AnalysisModel<ExpressionModel>, AnalysisForm, ExpressionModel>
{
    protected override string DataType => DataTypes.Omics.Rna.Expression;
    protected override IValidator<AnalysisModel<ExpressionModel>> ModelValidator => new AnalysisDataModelValidator<ExpressionModel>([AnalysisType.RNASeq], new ExpressionModelValidator());
    protected override IFormConverter<AnalysisModel<ExpressionModel>, AnalysisForm, ExpressionModel> FormConverter => new AnalysisFormConverter<ExpressionModel>();
    protected override IReader<ExpressionModel>[] EntryReaders =>
    [
        new TsvReader<ExpressionModel>(),
        new Models.Rna.Readers.DkfzRnaseq.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.RNA_EXP;


    public ExpressionsController(SubmissionTaskService submissionTaskService,
        ILogger<ExpressionsController> logger,
        Submissions.Repositories.Rna.ExpressionSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }
}
