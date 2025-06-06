using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Indices.Entities.Variants;

namespace Unite.Omics.Feed.Web.Controllers;

[Route("api/indexing")]
[Authorize(Policy = Policies.Data.Writer)]
public class IndexingController : Controller
{
    private readonly IIndexService<GeneIndex> _genesIndexService;
    private readonly IIndexService<SmIndex> _smsIndexService;
    private readonly IIndexService<CnvIndex> _cnvsIndexService;
    private readonly IIndexService<SvIndex> _svsIndexService;
    private readonly GeneIndexingTaskService _geneTasksService;
    private readonly SmIndexingTaskService _smTasksService;
    private readonly CnvIndexingTaskService _cnvTasksService;
    private readonly SvIndexingTaskService _svTasksService;
   


    public IndexingController(
        IIndexService<GeneIndex> genesIndexService,
        IIndexService<SmIndex> smsIndexService,
        IIndexService<CnvIndex> cnvsIndexService,
        IIndexService<SvIndex> svsIndexService,
        GeneIndexingTaskService geneTasksService,
        SmIndexingTaskService smTasksService,
        CnvIndexingTaskService cnvTasksService,
        SvIndexingTaskService svTasksService)
    {
        _genesIndexService = genesIndexService;
        _smsIndexService = smsIndexService;
        _cnvsIndexService = cnvsIndexService;
        _svsIndexService = svsIndexService;
        _geneTasksService = geneTasksService;
        _smTasksService = smTasksService;
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
        _smsIndexService.DeleteIndex().Wait();
        _cnvsIndexService.DeleteIndex().Wait();
        _svsIndexService.DeleteIndex().Wait();
        _smTasksService.CreateTasks();
        _cnvTasksService.CreateTasks();
        _svTasksService.CreateTasks();

        return Ok();
    }
}
