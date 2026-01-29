using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Base.Readers;
using Unite.Omics.Feed.Web.Models.Rna;
using Unite.Omics.Feed.Web.Models.Rna.Validators;
using Unite.Omics.Feed.Web.Submissions;
using Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

namespace Unite.Omics.Feed.Web.Controllers.Rna;

[Route("api/rna/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController(
    SubmissionTaskService submissionTaskService,
    ILogger<ExpressionsController> logger,
    Unite.Omics.Feed.Web.Submissions.Repositories.Rna.ExpSubmissionRepository submissionRepository)
    : AnalysisDataController<ExpressionModel>(submissionTaskService, logger)
{
    protected override IValidator<ExpressionModel> EntryModelValidator => new ExpressionModelValidator();
    protected override string DataType => DataTypes.Omics.Rna.Exp;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.RNASeq];
    protected override IReader<ExpressionModel>[] Readers =>
    [
        new TsvReader<ExpressionModel>(),
        new Models.Rna.Readers.DkfzRnaseq.Reader()
    ];

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.RNA_EXP;
    protected override SubmissionRepository SubmissionRepository => submissionRepository;
}
