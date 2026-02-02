using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

namespace Unite.Omics.Feed.Web.Controllers.RnaSc;

[Route("api/rnasc/analysis/exp")]
[Authorize(Policy = Policies.Data.Writer)]
public class ExpressionsController(
    SubmissionTaskService submissionTaskService,
    ILogger<ExpressionsController> logger,
    ExpSubmissionRepository submissionRepository)
    : AnalysisController<EmptyModel>(submissionTaskService, submissionRepository, logger)
{
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.RNASC_EXP;
    protected override string DataType => DataTypes.Omics.Rnasc.Exp;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.RNASeqSc, AnalysisType.RNASeqSn];

    protected override long AddSubmission(AnalysisModel<EmptyModel> model, bool review)
    {
        model.Resources?.ForEach(resource => resource.Type = DataType);

        return base.AddSubmission(model, review);
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
