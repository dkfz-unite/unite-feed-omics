using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Annotations.Services.Vep;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Data.Writers.Dna;
using Unite.Omics.Feed.Web.Handlers.Annotation.Converters;
using Unite.Omics.Feed.Web.Services.Indexing;

namespace Unite.Omics.Feed.Web.Handlers.Annotation;

public class CnvsAnnotationHandler
{
    private readonly CnvsAnnotationService _annotationService;
    private readonly EffectsCnvWriter _dataWriter;
    private readonly CnvIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly IGenomeOptions _genomeOptions;
    private readonly ILogger _logger;


    public CnvsAnnotationHandler(
        CnvsAnnotationService annotationService,
        EffectsCnvWriter dataWriter,
        CnvIndexingTaskService indexingTaskService,
        TasksProcessingService taskProcessingService,
        IGenomeOptions genomeOptions,
        ILogger<CnvsAnnotationHandler> logger)
    {
        _annotationService = annotationService;
        _dataWriter = dataWriter;
        _indexingTaskService = indexingTaskService;
        _taskProcessingService = taskProcessingService;
        _genomeOptions = genomeOptions;
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
        if (_taskProcessingService.HasTasks(WorkerType.Submission))
            return;
            
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(AnnotationTaskType.DNA_CNV, bucketSize, (tasks) =>
        {
            stopwatch.Restart();

            ProcessAnnotationTasks(tasks);

            stopwatch.Stop();

            _logger.LogInformation("Annotated {number} CNVs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessAnnotationTasks(Unite.Data.Entities.Tasks.Task[] tasks)
    {
        var variants = tasks.Select(task => int.Parse(task.Target)).ToArray();
        var annotations = _annotationService.Annotate(variants, _genomeOptions.Version);
        var data = EffectsDataConverter.Convert(annotations);

        _dataWriter.SaveData(data, out var audit);
        _indexingTaskService.PopulateTasks(audit.Variants);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
