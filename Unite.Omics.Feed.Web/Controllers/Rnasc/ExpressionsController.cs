using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.RnaSc;

[Route("api/rnasc/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController : AnalysisController
{
    private readonly RnaScSubmissionService _submissionService;
    private readonly SubmissionTaskService _submissionTaskService;
    protected override string DataType => DataTypes.Omics.Rnasc.Exp;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.RNASeqSc, AnalysisType.RNASeqSn];


    public ExpressionsController(
        RnaScSubmissionService submissionService,
        SubmissionTaskService submissionTaskService)
    {
        _submissionService = submissionService;
        _submissionTaskService = submissionTaskService;
    }


    protected override AnalysisModel<EmptyModel> GetSubmission(long id)
    {
        var task = _submissionTaskService.GetTask(id);

        return _submissionService.FindExpSubmission(task.Target);
    }

    protected override long AddSubmission(AnalysisModel<EmptyModel> model, bool review)
    {
        model.Resources?.ForEach(resource => resource.Type = DataType);

        var submissionId = _submissionService.AddExpSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        return _submissionTaskService.CreateTask(SubmissionTaskType.RNASC_EXP, submissionId, taskStatus);
    }

    // Temporarly commented as data source service submits resources one by one, not all together, so validation fails.
    // protected override void ValidateResources(ResourceModel[] resources, AnalysisModel<EmptyModel> model)
    // {
    //     base.ValidateResources(resources, model);

    //     var comparison = StringComparison.InvariantCultureIgnoreCase;

    //     var barcodes = resources.FirstOrDefault(resource =>
    //         resource.Format.Equals(FileTypes.General.Tsv, comparison) &&
    //         resource.Name.Equals("barcodes.tsv.gz", comparison));

    //     var features = resources.FirstOrDefault(resource =>
    //         resource.Format.Equals(FileTypes.General.Tsv, comparison) &&
    //         resource.Name.Equals("features.tsv.gz", comparison));

    //     var matrix = resources.FirstOrDefault(resource =>
    //         resource.Format.Equals(FileTypes.Sequence.Mtx, comparison) &&
    //         resource.Name.Equals("matrix.mtx.gz", comparison));

    //     if (barcodes == null || features == null || matrix == null)
    //     {
    //         ModelState.AddModelError("Resources", "barcodes.tsv.gz, features.tsv.gz, and matrix.mtx.gz files are required");
    //         Console.WriteLine(string.Join(Environment.NewLine, resources.Select(r => r.ToString())));
    //     }
    // }
}
