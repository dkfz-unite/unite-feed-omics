using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Controllers.Meth;

[Route("api/meth/sample")]
public class SampleController : Controllers.SampleController
{
    private readonly MethSubmissionService _submissionService;
    protected override string DataType => DataTypes.Omics.Meth.Sample;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.MethArray, AnalysisType.WGBS, AnalysisType.RRBS];


    public SampleController(MethSubmissionService submissionService, SubmissionTaskService submissionTaskService, ILogger<SampleController> logger) : base(submissionTaskService, logger)
    {
        _submissionService = submissionService;
    }


    protected override SampleModel FindSubmission(string id)
    {
        return _submissionService.FindSampleSubmission(id);
    }
    
    protected override long AddSubmission(SampleModel model, bool review)
    {
        var submissionId = _submissionService.AddSampleSubmission(model);

        var taskStatus = review ? TaskStatusType.Preparing : TaskStatusType.Prepared;

        return _submissionTaskService.CreateTask(SubmissionTaskType.METH, submissionId, taskStatus);
    }

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
