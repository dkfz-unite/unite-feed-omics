using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;
using Unite.Indices.Entities.Mutations;
using Unite.Indices.Services;
using Unite.Mutations.Feed.Indices.Services;

namespace Unite.Mutations.Feed.Web.Handlers
{
    public class IndexingHandler
    {
        private readonly UniteDbContext _dbContext;
        private readonly MutationIndexCreationService _indexCreationService;
        private readonly IIndexingService<MutationIndex> _indexingService;
        private readonly ILogger _logger;

        public IndexingHandler(
            UniteDbContext dbContext,
            MutationIndexCreationService indexCreationService,
            IIndexingService<MutationIndex> indexingService,
            ILogger<IndexingHandler> logger)
        {
            _dbContext = dbContext;
            _indexCreationService = indexCreationService;
            _indexingService = indexingService;
            _logger = logger;
        }

        public void Handle(int bucketSize)
        {
            try
            {
                ProcessMutationIndexingTasks(bucketSize);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);

                if(exception.InnerException != null)
                {
                    _logger.LogError(exception.InnerException.Message);
                }
            }
        }


        private void ProcessMutationIndexingTasks(int bucketSize)
        {
            while (_dbContext.Tasks.Any(IsMutationIndexingTask))
            {
                var tasks = _dbContext.Tasks
                    .Where(IsMutationIndexingTask)
                    .OrderByDescending(task => task.Date)
                    .Take(bucketSize)
                    .ToArray();

                _logger.LogInformation($"Indexing {tasks.Length} mutations");

                var indices = tasks.Select(task =>
                {
                    var id = int.Parse(task.Target);
                    var index = _indexCreationService.CreateIndex(id);

                    return index;

                }).ToArray();

                _indexingService.IndexMany(indices);

                _dbContext.Tasks.RemoveRange(tasks);
                _dbContext.SaveChanges();

                _logger.LogInformation($"Indexing of {tasks.Length} mutations completed");
            }
        }

        private bool IsMutationIndexingTask(Task task)
        {
            return task.TypeId == TaskType.Indexing && task.TargetTypeId == TaskTargetType.Mutation;
        }
    }
}
