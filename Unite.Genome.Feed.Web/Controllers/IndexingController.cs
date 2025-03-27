using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Indices.Entities.Variants;

namespace Unite.Genome.Feed.Web.Controllers;

[Route("api/indexing")]
[Authorize(Policy = Policies.Data.Writer)]
public class IndexingController : Controller
{
    private readonly IIndexService<GeneIndex> _genesIndexService;
    private readonly IIndexService<SsmIndex> _ssmsIndexService;
    private readonly IIndexService<CnvIndex> _cnvsIndexService;
    private readonly IIndexService<SvIndex> _svsIndexService;
    private readonly GeneIndexingTaskService _geneTasksService;
    private readonly SsmIndexingTaskService _ssmTasksService;
    private readonly CnvIndexingTaskService _cnvTasksService;
    private readonly SvIndexingTaskService _svTasksService;
   


    public IndexingController(
        IIndexService<GeneIndex> genesIndexService,
        IIndexService<SsmIndex> ssmsIndexService,
        IIndexService<CnvIndex> cnvsIndexService,
        IIndexService<SvIndex> svsIndexService,
        GeneIndexingTaskService geneTasksService,
        SsmIndexingTaskService ssmTasksService,
        CnvIndexingTaskService cnvTasksService,
        SvIndexingTaskService svTasksService)
    {
        _genesIndexService = genesIndexService;
        _ssmsIndexService = ssmsIndexService;
        _cnvsIndexService = cnvsIndexService;
        _svsIndexService = svsIndexService;
        _geneTasksService = geneTasksService;
        _ssmTasksService = ssmTasksService;
        _cnvTasksService = cnvTasksService;
        _svTasksService = svTasksService;
    }


    [HttpPost("genes")]
    public IActionResult Genes()
    {
        _genesIndexService.DeleteIndex().Wait();
        _geneTasksService.CreateTasks();

        return Ok();
    }

    [HttpPost("variants")]
    public IActionResult Variants()
    {
        _ssmsIndexService.DeleteIndex().Wait();
        _cnvsIndexService.DeleteIndex().Wait();
        _svsIndexService.DeleteIndex().Wait();
        _ssmTasksService.CreateTasks();
        _cnvTasksService.CreateTasks();
        _svTasksService.CreateTasks();

        return Ok();
    }
}
