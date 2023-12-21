using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Services.Indexing;

namespace Unite.Genome.Feed.Web.Controllers;

[Route("api/[controller]/[action]")]
[Authorize(Policy = Policies.Data.Writer)]
public class IndexingController : Controller
{
    private readonly GeneIndexingTaskService _geneIndexingTaskService;
    private readonly SsmIndexingTaskService _ssmIndexingTaskService;
    private readonly CnvIndexingTaskService _cnvIndexingTaskService;
    private readonly SvIndexingTaskService _svIndexingTaskService;

    public IndexingController(
        GeneIndexingTaskService geneIndexingTaskService,
        SsmIndexingTaskService ssmIndexingTaskService,
        CnvIndexingTaskService cnvIndexingTaskService,
        SvIndexingTaskService svIndexingTaskService)
    {
        _geneIndexingTaskService = geneIndexingTaskService;
        _ssmIndexingTaskService = ssmIndexingTaskService;
        _cnvIndexingTaskService = cnvIndexingTaskService;
        _svIndexingTaskService = svIndexingTaskService;
    }


    [HttpPost]
    public IActionResult Genes()
    {
        _geneIndexingTaskService.CreateTasks();

        return Ok();
    }

    [HttpPost]
    public IActionResult Variants()
    {
        _ssmIndexingTaskService.CreateTasks();
        _cnvIndexingTaskService.CreateTasks();
        _svIndexingTaskService.CreateTasks();

        return Ok();
    }
}
