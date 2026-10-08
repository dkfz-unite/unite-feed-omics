using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Submissions.Repositories.Prot;

namespace Unite.Omics.Feed.Web.Controllers.Prot;

[Route("api/prot/sample")]
public class SampleController : Controllers.SampleController
{
    public SampleController(SubmissionTaskService submissionTaskService,
        ILogger<SampleController> logger,
        SampleSubmissionRepository submissionRepository) : base(submissionTaskService, submissionRepository, logger)
    {
    }

    protected override SubmissionTaskType SubmissionTaskType => SubmissionTaskType.PROT;
    protected override string DataType => DataTypes.Omics.Proteomics.Sample;
    protected override AnalysisType[] AnalysisTypes => [AnalysisType.MS];

    protected override void ValidateModel(SampleModel model)
    {
        base.ValidateModel(model);

        // JSON submissions carry resources in the model; form submissions read them later.
        ValidateResourceFormats(model.Resources);
    }

    protected override void ValidateResources(ResourceModel[] resources)
    {
        base.ValidateResources(resources);

        ValidateResourceFormats(resources);
    }

    private void ValidateResourceFormats(ResourceModel[] resources)
    {
        // Only the open mzML format is supported, vendor-specific raw formats are not.
        if (resources?.Any(resource => resource.Format != FileTypes.Sequence.Mzml) == true)
        {
            ModelState.AddModelError("Resources", $"Allowed formats are [{FileTypes.Sequence.Mzml}]");
        }
    }
}
