using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Web.Services;

namespace Unite.Genome.Feed.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IndexingController : Controller
    {
        private readonly GeneIndexingTaskService _geneIndexingTaskService;
        private readonly MutationIndexingTaskService _mutationIndexingTaskService;


        public IndexingController(
            GeneIndexingTaskService geneIndexingTaskService,
            MutationIndexingTaskService mutationIndexingTaskService)
        {
            _geneIndexingTaskService = geneIndexingTaskService;
            _mutationIndexingTaskService = mutationIndexingTaskService;
        }


        [HttpPost]
        public IActionResult Genes()
        {
            _geneIndexingTaskService.CreateTasks();

            return Ok();
        }

        [HttpPost]
        public IActionResult Mutations()
        {
            _mutationIndexingTaskService.CreateTasks();

            return Ok();
        }
    }
}
