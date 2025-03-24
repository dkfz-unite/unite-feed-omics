using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Data.Writers;
using Unite.Genome.Feed.Web.Services.Indexing;

namespace Unite.Genome.Feed.Web.Controllers.Rna;

[Route("api/rna/sample")]
public class SampleController : Controllers.SampleController
{
    public SampleController(SampleWriter dataWriter, SampleIndexingTaskService taskService, ILogger<SampleController> logger) : base(dataWriter, taskService, logger)
    {
    }
}
