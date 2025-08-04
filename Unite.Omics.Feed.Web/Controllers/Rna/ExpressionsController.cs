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
using Unite.Omics.Feed.Web.Models.Base.Validators;
using Unite.Omics.Feed.Web.Models.Rna;
using Unite.Omics.Feed.Web.Models.Rna.Validators;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.Rna;

[Route("api/rna/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController : AnalysisDataController<ExpressionModel>
{
    private readonly RnaSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;

    protected override IValidator<ExpressionModel> EntryModelValidator => new ExpressionModelValidator();
    protected override IValidator<ResourceModel> ResourceModelValidator => new ResourceModelValidator();
    protected override string DataType => DataTypes.Omics.Rna.Exp;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.RNASeq];
    protected override IReader<ExpressionModel>[] Readers =>
    [
        new Models.Rna.Readers.Tsv.Reader(),
        new Models.Rna.Readers.DkfzRnaseq.Reader()
    ]; 


	public ExpressionsController(
        RnaSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }


    protected override AnalysisModel<ExpressionModel> GetSubmission(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        return _submissionService.FindExpSubmission(task.Target);
    }

    protected override long AddSubmission(AnalysisModel<ExpressionModel> model, bool review)
    {
        var submissionId = _submissionService.AddExpSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        return _submissionTaskService.CreateTask(SubmissionTaskType.RNA_EXP, submissionId, taskStatus);
    }
}
