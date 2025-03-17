using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Unite.Genome.Feed.Data.Writers;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Services.Indexing;

namespace Unite.Genome.Feed.Web.Controllers.Meth;

[Route("api/meth/sample")]
[Authorize(Policy = Policies.Data.Writer)]
public class SampleController : Controllers.SampleController
{
    public SampleController(SampleWriter dataWriter, SampleIndexingTaskService taskService, ILogger<SampleController> logger) : base(dataWriter, taskService, logger)
    {
    }
}
