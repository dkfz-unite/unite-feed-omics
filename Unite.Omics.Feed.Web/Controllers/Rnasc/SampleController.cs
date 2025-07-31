using Microsoft.AspNetCore.Mvc;
using Unite.Data.Constants;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Services.Indexing;

namespace Unite.Omics.Feed.Web.Controllers.RnaSc;

[Route("api/rnasc/sample")]
public class SampleController : Controllers.SampleController
{
    protected override string DataType => DataTypes.Omics.Rnasc.Sample;

    public SampleController(SampleWriter dataWriter, SampleIndexingTaskService taskService, ILogger<SampleController> logger) : base(dataWriter, taskService, logger)
    {
    }

    protected override void ValidateResources(ResourceModel[] resources)
    {
        base.ValidateResources(resources);

        var barcodes = resources.FirstOrDefault(resource =>
            resource.Format == FileTypes.General.Tsv &&
            resource.Name.Equals("barcodes.tsv.gz", StringComparison.InvariantCultureIgnoreCase));

        var features = resources.FirstOrDefault(resource =>
            resource.Format == FileTypes.General.Tsv &&
            resource.Name.Equals("features.tsv.gz", StringComparison.InvariantCultureIgnoreCase));

        var matrix = resources.FirstOrDefault(resource =>
            resource.Format == FileTypes.Sequence.Mtx &&
            resource.Name.Equals("matrix.mtx.gz", StringComparison.InvariantCultureIgnoreCase));

        if (barcodes == null || features == null || matrix == null)
            ModelState.AddModelError("Resources", "barcodes.tsv.gz, features.tsv.gz, and matrix.mtx.gz files are required");
    }
}
