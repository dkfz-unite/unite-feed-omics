using System.Linq;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Annotations.Services;
using Unite.Genome.Feed.Web.Services;

namespace Unite.Genome.Feed.Web.Handlers
{
    public class MutationsAnnotationHandler
    {
        private readonly TasksProcessingService _taskProcessingService;
        private readonly AnnotationService _annotationService;
        private readonly MutationIndexingTaskService _indexingTaskService;
        private readonly ILogger _logger;


        public MutationsAnnotationHandler(
            TasksProcessingService taskProcessingService,
            AnnotationService annotationService,
            MutationIndexingTaskService indexingTaskService,
            ILogger<MutationsAnnotationHandler> logger)
        {
            _taskProcessingService = taskProcessingService;
            _annotationService = annotationService;
            _indexingTaskService = indexingTaskService;
            _logger = logger;
        }


        public void Handle(int bucketSize)
        {
            ProcessAnnotationTasks(bucketSize);
        }


        private void ProcessAnnotationTasks(int bucketSize)
        {
            _taskProcessingService.Process(TaskType.Annotation, TaskTargetType.Mutation, bucketSize, (tasks) =>
            {
                var mutationIds = tasks
                    .Select(task => long.Parse(task.Target))
                    .ToArray();

                _annotationService.Annotate(mutationIds, out var audit);

                _indexingTaskService.PopulateTasks(audit.Mutations);

                _logger.LogInformation(audit.ToString());

                _logger.LogInformation($"Finished annotation of {tasks.Length} mutations");
            });
        }
    }
}
