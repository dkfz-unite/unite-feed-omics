using System.Diagnostics;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Genome.Annotations.Services.Vep;
using Unite.Genome.Feed.Web.Services;

namespace Unite.Genome.Feed.Web.Handlers;

public class CopyNumberVariantsAnnotationHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly CopyNumberVariantsAnnotationService _annotationService;
    private readonly CopyNumberVariantIndexingTaskService _indexingTaskService;
    private readonly ILogger _logger;


    public CopyNumberVariantsAnnotationHandler(
        TasksProcessingService taskProcessingService,
        CopyNumberVariantsAnnotationService annotationService,
        CopyNumberVariantIndexingTaskService indexingTaskService,
        ILogger<CopyNumberVariantsAnnotationHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _annotationService = annotationService;
        _indexingTaskService = indexingTaskService;
        _logger = logger;
    }


    public void Prepare()
    {

    }

    public void Handle(int bucketSize)
    {
        ProcessAnnotationTasks(bucketSize);
    }


    private void ProcessAnnotationTasks(int bucketSize)
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(AnnotationTaskType.CNV, bucketSize, (tasks) =>
        {
            _logger.LogInformation($"Annotating {tasks.Length} copy number variants");

            stopwatch.Restart();

            var variantIds = tasks
                .Select(task => long.Parse(task.Target))
                .ToArray();

            _annotationService.Annotate(variantIds, out var audit);

            _indexingTaskService.PopulateTasks(audit.Variants);

            _logger.LogInformation(audit.ToString());

            stopwatch.Stop();

            _logger.LogInformation($"Annotation of {tasks.Length} copy number variants completed in {Math.Round(stopwatch.Elapsed.TotalSeconds, 2)}s");
        });
    }
}
