using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Services;
using Unite.Indices.Entities.Mutations;
using Unite.Indices.Services;
using Unite.Mutations.DataFeed.Web.Services.Indices;

namespace Unite.Mutations.DataFeed.Web.Services
{
    public class TaskProcessingService : ITaskProcessingService
    {
        private readonly UniteDbContext _database;
        private readonly IIndexCreationService _indexCreationService;
        private readonly IIndexingService<MutationIndex> _indexingService;
        private readonly ILogger _logger;

        public TaskProcessingService(
            UniteDbContext database,
            IIndexCreationService indexCreationService,
            IIndexingService<MutationIndex> indexingService,
            ILogger<TaskProcessingService> logger)
        {
            _database = database;
            _indexCreationService = indexCreationService;
            _indexingService = indexingService;
            _logger = logger;
        }

        public void ProcessIndexingTasks(int bucketSize)
        {
            while (_database.MutationIndexingTasks.Any())
            {
                var tasks = _database.MutationIndexingTasks
                    .OrderBy(task => task.Date)
                    .Take(bucketSize)
                    .ToArray();

                var indices = new List<MutationIndex>();

                foreach (var task in tasks)
                {
                    var index = _indexCreationService.CreateIndex(task.MutationId);

                    if (index != null)
                    {
                        indices.Add(index);
                    }
                }

                _logger.LogInformation($"Starting to index {indices.Count()} mutations");

                _indexingService.IndexMany(indices);

                _database.MutationIndexingTasks.RemoveRange(tasks);

                _database.SaveChanges();

                _logger.LogInformation($"Indexing of {indices.Count()} mutations completed");
            }
        }
    }
}
