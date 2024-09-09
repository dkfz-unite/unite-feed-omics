using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Annotations.Services.Vep;
using Unite.Genome.Feed.Data.Writers.Dna;
using Unite.Genome.Feed.Web.Handlers.Annotation.Converters;
using Unite.Genome.Feed.Web.Services.Indexing;

namespace Unite.Genome.Feed.Web.Handlers.Annotation;

public class SsmsAnnotationHandler
{
    private readonly SsmsAnnotationService _annotationService;
    private readonly EffectsSsmWriter _dataWriter;
    private readonly SsmIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;


    public SsmsAnnotationHandler(
        SsmsAnnotationService annotationService,
        EffectsSsmWriter dataWriter,
        SsmIndexingTaskService indexingTaskService,
        TasksProcessingService taskProcessingService,
        ILogger<SsmsAnnotationHandler> logger)
    {
        _annotationService = annotationService;
        _dataWriter = dataWriter;
        _indexingTaskService = indexingTaskService;
        _taskProcessingService = taskProcessingService;
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

        _taskProcessingService.Process(AnnotationTaskType.DNA_SSM, bucketSize, (tasks) =>
        {
            if (_taskProcessingService.HasTasks(WorkerType.Submission))
                return false;

            stopwatch.Restart();

            ProcessAnnotationTasks(tasks);

            stopwatch.Stop();

            _logger.LogInformation("Annotated {number} SSMs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessAnnotationTasks(Unite.Data.Entities.Tasks.Task[] tasks)
    {
        var variants = tasks.Select(task => int.Parse(task.Target)).ToArray();
        var annotations = _annotationService.Annotate(variants);
        var data = EffectsDataConverter.Convert(annotations);

        _dataWriter.SaveData(data, out var audit);
        _indexingTaskService.PopulateTasks(audit.Variants);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
