using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Mutations.Annotations.Vep.Services;
using Unite.Mutations.Feed.Web.Services;

namespace Unite.Mutations.Feed.Web.Handlers
{
    public class AnnotationHandler
    {
        private readonly TaskProcessingService _taskProcessingService;
        private readonly VepAnnotationService _vepAnnotationService;
        private readonly MutationIndexingTaskService _mutationIndexingTaskService;
        private readonly ILogger _logger;


        public AnnotationHandler(
            TaskProcessingService taskProcessingService,
            VepAnnotationService vepAnnotationService,
            MutationIndexingTaskService mutationIndexingTaskService,
            ILogger<AnnotationHandler> logger)
        {
            _taskProcessingService = taskProcessingService;
            _vepAnnotationService = vepAnnotationService;
            _mutationIndexingTaskService = mutationIndexingTaskService;
            _logger = logger;
        }


        public void Handle(int bucketSize)
        {
            ProcessMutationAnnotationTasks(bucketSize);
        }


        private void ProcessMutationAnnotationTasks(int bucketSize)
        {
            _taskProcessingService.Process(TaskType.Annotation, TaskTargetType.Mutation, bucketSize, (tasks) =>
            {
                _logger.LogInformation($"Annotating {tasks.Length} mutations");

                var mutationIds = tasks
                    .Select(task => long.Parse(task.Target))
                    .ToArray();

                _vepAnnotationService.Annotate(mutationIds, out var audit);

                _mutationIndexingTaskService.PopulateTasks(audit.Mutations);

                _logger.LogInformation(audit.ToString());

                _logger.LogInformation($"Annotation of {tasks.Length} mutations completed");
            });
        }
    }
}
