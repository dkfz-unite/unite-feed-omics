using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Omics.Feed.Web.Configuration.Constants;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Indices.Entities.Variants;
using Unite.Indices.Entities.Proteins;

namespace Unite.Omics.Feed.Web.Controllers;

[Route("api/indexing")]
[Authorize(Policy = Policies.Data.Writer)]
public class IndexingController : Controller
{
    private readonly IIndexService<GeneIndex> _genesIndexService;
    private readonly IIndexService<ProteinIndex> _proteinsIndexService;
    private readonly IIndexService<SmIndex> _smsIndexService;
    private readonly IIndexService<CnvIndex> _cnvsIndexService;
    private readonly IIndexService<SvIndex> _svsIndexService;
    private readonly GeneIndexingTaskService _geneTasksService;
    private readonly ProteinIndexingTaskService _proteinTasksService;
    private readonly SmIndexingTaskService _smTasksService;
    private readonly CnvIndexingTaskService _cnvTasksService;
    private readonly SvIndexingTaskService _svTasksService;
   


    public IndexingController(
        IIndexService<GeneIndex> genesIndexService,
        IIndexService<ProteinIndex> proteinsIndexService,
        IIndexService<SmIndex> smsIndexService,
        IIndexService<CnvIndex> cnvsIndexService,
        IIndexService<SvIndex> svsIndexService,
        GeneIndexingTaskService geneTasksService,
        ProteinIndexingTaskService proteinTasksService,
        SmIndexingTaskService smTasksService,
        CnvIndexingTaskService cnvTasksService,
        SvIndexingTaskService svTasksService)
    {
        _genesIndexService = genesIndexService;
        _proteinsIndexService = proteinsIndexService;
        _smsIndexService = smsIndexService;
        _cnvsIndexService = cnvsIndexService;
        _svsIndexService = svsIndexService;
        _geneTasksService = geneTasksService;
        _proteinTasksService = proteinTasksService;
        _smTasksService = smTasksService;
        _cnvTasksService = cnvTasksService;
        _svTasksService = svTasksService;
    }


    [HttpPost("genes")]
    public async Task<IActionResult> Genes()
    {
        await DeleteIndex(_genesIndexService.DeleteIndex());
        
        _geneTasksService.CreateTasks();

        return Ok();
    }

    [HttpPost("proteins")]
    public async Task<IActionResult> Proteins()
    {
        await DeleteIndex(_proteinsIndexService.DeleteIndex());

        _proteinTasksService.CreateTasks();

        return Ok();
    }

    [HttpPost("variants")]
    public async Task<IActionResult> Variants()
    {
        await DeleteIndex(_smsIndexService.DeleteIndex());
        await DeleteIndex(_cnvsIndexService.DeleteIndex());
        await DeleteIndex(_svsIndexService.DeleteIndex());

        _smTasksService.CreateTasks();
        _cnvTasksService.CreateTasks();
        _svTasksService.CreateTasks();

        return Ok();
    }


    private static async Task DeleteIndex(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            // Ignore errors
        }
    }
}
