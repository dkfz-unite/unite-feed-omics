using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Controllers
{
    [Route("api/[controller]")]
    public class IndicesController : Controller
    {
        private readonly UniteDbContext _database;
        private readonly ILogger _logger;

        public IndicesController(UniteDbContext database, ILogger<IndicesController> logger)
        {
            _database = database;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult Post()
        {
            var mutations = _database.Mutations
                .Select(mutation => mutation.Id)
                .ToArray();

            var tasks = mutations
                .Select(CreateTask)
                .ToArray();

            _logger.LogInformation($"Adding indexing tasks for {mutations.Length} mutations");

            _database.MutationIndexingTasks.AddRange(tasks);
            _database.SaveChanges();

            return Ok();
        }

        private MutationIndexingTask CreateTask(int mutationId)
        {
            var task = new MutationIndexingTask()
            {
                MutationId = mutationId,
                Date = DateTime.UtcNow
            };

            return task;
        }
    }
}
