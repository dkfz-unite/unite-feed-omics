using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

namespace Unite.Omics.Feed.Web.Controllers.Meth;

[Route("api/meth/sample")]
public class SampleController(
    SubmissionTaskService submissionTaskService,
    ILogger<SampleController> logger,
    SampleSubmissionRepository submissionRepository)
    : Controllers.SampleController(submissionTaskService, submissionRepository, logger)
{
    protected override string DataType => DataTypes.Omics.Methylation.Sample;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.MethArray, AnalysisType.WGBS, AnalysisType.RRBS];
    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.METH;

    // Temporarly commented as data source service submits resources one by one, not all together, so validation fails.
    // protected override void ValidateResources(ResourceModel[] resources)
    // {
    //     base.ValidateResources(resources);

    //     if (resources.Any(resource => resource.Format == FileTypes.Sequence.Idat))
    //     {
    //         var comparison = StringComparison.InvariantCultureIgnoreCase;

    //         var red = resources.FirstOrDefault(resource =>
    //             resource.Format.Equals(FileTypes.Sequence.Idat, comparison) &&
    //             resource.Name.EndsWith("Red.idat", comparison));

    //         var grn = resources.FirstOrDefault(resource =>
    //             resource.Format.Equals(FileTypes.Sequence.Idat, comparison) &&
    //             resource.Name.EndsWith("Red.idat", comparison));

    //         if (red == null || grn == null)
    //         {
    //             ModelState.AddModelError("Resources", "Red.idat and Grn.idat files are required");
    //             Console.WriteLine(string.Join(Environment.NewLine, resources.Select(r => r.ToString())));
    //         }       
    //     }
    // }
}
