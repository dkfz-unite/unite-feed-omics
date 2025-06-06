using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Annotations.Services.Vep;
using Unite.Omics.Feed.Data.Writers.Dna;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers.Annotation.Converters;
using Unite.Omics.Feed.Web.Services.Indexing;

namespace Unite.Omics.Feed.Web.Handlers.Annotation;

public class SmsAnnotationHandler
{
    private readonly SmsAnnotationService _annotationService;
    private readonly EffectsSmWriter _dataWriter;
    private readonly SmIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;


    public SmsAnnotationHandler(
        SmsAnnotationService annotationService,
        EffectsSmWriter dataWriter,
        SmIndexingTaskService indexingTaskService,
        TasksProcessingService taskProcessingService,
        ILogger<SmsAnnotationHandler> logger)
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

        _taskProcessingService.Process(AnnotationTaskType.DNA_SM, bucketSize, (tasks) =>
        {
            if (_taskProcessingService.HasTasks(WorkerType.Submission))
                return false;

            stopwatch.Restart();

            ProcessAnnotationTasks(tasks);

            stopwatch.Stop();

            _logger.LogInformation("Annotated {number} SMs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessAnnotationTasks(Unite.Data.Entities.Tasks.Task[] tasks)
    {
        var variants = tasks.Select(task => int.Parse(task.Target)).ToArray();
        var annotations = _annotationService.Annotate(variants, GenomeOptions.Version);
        var data = EffectsDataConverter.Convert(annotations);

        _dataWriter.SaveData(data, out var audit);
        _indexingTaskService.PopulateTasks(audit.Variants);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
