using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Web.Configuration.Constants;
using Unite.Genome.Feed.Web.Services.Indexing;

namespace Unite.Genome.Feed.Web.Controllers;

[Route("api/[controller]/[action]")]
[Authorize(Roles = Roles.Admin)]
public class IndexingController : Controller
{
    private readonly GeneIndexingTaskService _geneIndexingTaskService;
    private readonly MutationIndexingTaskService _mutationIndexingTaskService;
    private readonly CopyNumberVariantIndexingTaskService _copyNumberVariantIndexingTaskService;
    private readonly StructuralVariantIndexingTaskService _structuralVariantIndexingTaskService;

    public IndexingController(
        GeneIndexingTaskService geneIndexingTaskService,
        MutationIndexingTaskService mutationIndexingTaskService,
        CopyNumberVariantIndexingTaskService copyNumberVariantIndexingTaskService,
        StructuralVariantIndexingTaskService structuralVariantIndexingTaskService)
    {
        _geneIndexingTaskService = geneIndexingTaskService;
        _mutationIndexingTaskService = mutationIndexingTaskService;
        _copyNumberVariantIndexingTaskService = copyNumberVariantIndexingTaskService;
        _structuralVariantIndexingTaskService = structuralVariantIndexingTaskService;
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
        _mutationIndexingTaskService.CreateTasks();
        _copyNumberVariantIndexingTaskService.CreateTasks();
        _structuralVariantIndexingTaskService.CreateTasks();

        return Ok();
    }
}
