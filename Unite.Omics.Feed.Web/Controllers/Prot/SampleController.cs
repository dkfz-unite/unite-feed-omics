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

        ValidateResourceFormats(model.Resources);
    }

    protected override void ValidateResources(ResourceModel[] resources)
    {
        base.ValidateResources(resources);

        ValidateResourceFormats(resources);
    }

    private void ValidateResourceFormats(ResourceModel[] resources)
    {
        var mzmlFile = resources?.FirstOrDefault(resource => resource.Format == FileTypes.Sequence.Mzml);
        if (mzmlFile == null)
            ModelState.AddModelError("Resources", $"At least one resource must be in the format [{FileTypes.Sequence.Mzml}]");
    }
}
