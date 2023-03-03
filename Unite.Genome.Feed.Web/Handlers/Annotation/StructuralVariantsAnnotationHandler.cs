using System.Diagnostics;
using Unite.Data.Entities.Genome.Variants.SV;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;
using Unite.Data.Services.Tasks;
using Unite.Genome.Annotations.Services.Vep;
using Unite.Genome.Feed.Data;
using Unite.Genome.Feed.Data.Models.Variants.SV;
using Unite.Genome.Feed.Data.Repositories.Variants.SV;
using Unite.Genome.Feed.Web.Handlers.Annotation.Converters;
using Unite.Genome.Feed.Web.Services.Indexing;

namespace Unite.Genome.Feed.Web.Handlers.Annotation;

public class StructuralVariantsAnnotationHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly StructuralVariantsAnnotationService _annotationService;
    private readonly StructuralVariantIndexingTaskService _indexingTaskService;
    private readonly ConsequencesDataWriter<AffectedTranscript, Variant, VariantModel> _dataWriter;
    private readonly ILogger _logger;


    public StructuralVariantsAnnotationHandler(
        DomainDbContext dbContext,
        TasksProcessingService taskProcessingService,
        StructuralVariantsAnnotationService annotationService,
        StructuralVariantIndexingTaskService indexingTaskService,
        ILogger<StructuralVariantsAnnotationHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _annotationService = annotationService;
        _indexingTaskService = indexingTaskService;
        _logger = logger;

        var variantRepository = new VariantRepository(dbContext);
        var affectedTranscriptRepository = new AffectedTranscriptRepository(dbContext, variantRepository);
        _dataWriter = new ConsequencesDataWriter<AffectedTranscript, Variant, VariantModel>(dbContext, variantRepository, affectedTranscriptRepository);
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

        _taskProcessingService.Process(AnnotationTaskType.SV, bucketSize, (tasks) =>
        {
            if (_taskProcessingService.HasSubmissionTasks())
            {
                return false;
            }

            _logger.LogInformation($"Annotating {tasks.Length} structural variants");

            stopwatch.Restart();

            var variantIds = tasks.Select(task => long.Parse(task.Target)).ToArray();

            var annotations = _annotationService.Annotate(variantIds);

            var consequences = ConsequencesDataConverter.Convert(annotations);

            _dataWriter.SaveData(consequences, out var audit);

            _indexingTaskService.PopulateTasks(audit.Variants);

            _logger.LogInformation(audit.ToString());

            stopwatch.Stop();

            _logger.LogInformation($"Annotation of {tasks.Length} structural variants completed in {Math.Round(stopwatch.Elapsed.TotalSeconds, 2)}s");

            return true;
        });
    }
}
