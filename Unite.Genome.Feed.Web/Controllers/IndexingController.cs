using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Services.Indexing;

namespace Unite.Genome.Feed.Web.Controllers;

[Route("api/indexing")]
[Authorize(Policy = Policies.Data.Writer)]
public class IndexingController : Controller
{
    private readonly SsmIndexingTaskService _ssmTasksService;
    private readonly CnvIndexingTaskService _cnvTasksService;
    private readonly SvIndexingTaskService _svTasksService;
    private readonly GeneIndexingTaskService _geneTasksService;


    public IndexingController(
        SsmIndexingTaskService ssmTasksService,
        CnvIndexingTaskService cnvTasksService,
        SvIndexingTaskService svTasksService,
        GeneIndexingTaskService geneTasksService)
    {
        _ssmTasksService = ssmTasksService;
        _cnvTasksService = cnvTasksService;
        _svTasksService = svTasksService;
        _geneTasksService = geneTasksService;
    }


    [HttpPost("variants")]
    public IActionResult Variants()
    {
        _ssmTasksService.CreateTasks();
        _cnvTasksService.CreateTasks();
        _svTasksService.CreateTasks();

        return Ok();
    }

    [HttpPost("genes")]
    public IActionResult Genes()
    {
        _geneTasksService.CreateTasks();

        return Ok();
    }
}
