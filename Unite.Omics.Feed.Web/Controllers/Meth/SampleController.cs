using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Services.Indexing;

namespace Unite.Omics.Feed.Web.Controllers.Meth;

[Route("api/meth/sample")]
public class SampleController : Controllers.SampleController
{
    protected override string DataType => DataTypes.Omics.Meth.Sample;

    public SampleController(SampleWriter dataWriter, SampleIndexingTaskService taskService, ILogger<SampleController> logger) : base(dataWriter, taskService, logger)
    {
    }

    protected override void ValidateResources(ResourceModel[] resources)
    {
        base.ValidateResources(resources);

        if (resources.Any(resource => resource.Format == FileTypes.Sequence.Idat))
        {
            var red = resources.FirstOrDefault(resource =>
                resource.Format == FileTypes.Sequence.Idat &&
                resource.Name.EndsWith("Red.idat", StringComparison.InvariantCultureIgnoreCase));

            var grn = resources.FirstOrDefault(resource =>
                resource.Format == FileTypes.Sequence.Idat &&
                resource.Name.EndsWith("Grn.idat", StringComparison.InvariantCultureIgnoreCase));

            if (red == null || grn == null)
                ModelState.AddModelError("Resources", "Red.idat and Grn.idat files are required");
        }
    }
}
