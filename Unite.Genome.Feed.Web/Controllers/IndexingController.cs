using Microsoft.AspNetCore.Mvc;
using Unite.Genome.Feed.Web.Services;

namespace Unite.Genome.Feed.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class IndexingController : Controller
    {
        private readonly MutationIndexingTaskService _indexingTaskService;


        public IndexingController(MutationIndexingTaskService indexingTaskService)
        {
            _indexingTaskService = indexingTaskService;
        }


        [HttpPost]
        public IActionResult Mutations()
        {
            _indexingTaskService.CreateTasks();

            return Ok();
        }
    }
}
