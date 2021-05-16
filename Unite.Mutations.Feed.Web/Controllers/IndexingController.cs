using Microsoft.AspNetCore.Mvc;
using Unite.Mutations.Feed.Web.Services;

namespace Unite.Mutations.Feed.Web.Controllers
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
