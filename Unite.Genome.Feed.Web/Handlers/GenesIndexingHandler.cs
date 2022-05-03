using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Web.Services;
using Unite.Indices.Entities.Genes;
using Unite.Indices.Services;

namespace Unite.Genome.Feed.Web.Handlers
{
    public class GenesIndexingHandler
    {
        private readonly TasksProcessingService _taskProcessingService;
        private readonly IIndexCreationService<GeneIndex> _indexCreationService;
        private readonly IIndexingService<GeneIndex> _indexingService;
        private readonly ILogger _logger;


        public GenesIndexingHandler(
            TasksProcessingService taskProcessingService,
            IIndexCreationService<GeneIndex> indexCreationService,
            IIndexingService<GeneIndex> indexingService,
            ILogger<GenesIndexingHandler> logger)
        {
            _taskProcessingService = taskProcessingService;
            _indexCreationService = indexCreationService;
            _indexingService = indexingService;
            _logger = logger;
        }


        public void Prepare()
        {
            _indexingService.UpdateMapping().GetAwaiter().GetResult();
        }

        public void Handle(int bucketSize)
        {
            ProcessIndexingTasks(bucketSize);
        }


        private void ProcessIndexingTasks(int bucketSize)
        {
            _taskProcessingService.Process(TaskType.Indexing, TaskTargetType.Gene, bucketSize, (tasks) =>
            {
                _logger.LogInformation($"Indexing {tasks.Length} genes");

                var indices = tasks.Select(task =>
                {
                    var id = int.Parse(task.Target);

                    var index = _indexCreationService.CreateIndex(id);

                    return index;

                }).ToArray();

                _indexingService.IndexMany(indices);

                _logger.LogInformation($"Indexing of {tasks.Length} genes completed");
            });
        }
    }
}
