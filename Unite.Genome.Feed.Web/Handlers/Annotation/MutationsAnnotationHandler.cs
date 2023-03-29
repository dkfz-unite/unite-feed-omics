using System.Diagnostics;
using Unite.Data.Entities.Genome.Variants.SSM;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Data.Services.Tasks;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Services.Vep;
using Unite.Genome.Feed.Data;
using Unite.Genome.Feed.Data.Models.Variants.SSM;
using Unite.Genome.Feed.Data.Repositories.Variants.SSM;
using Unite.Genome.Feed.Web.Handlers.Annotation.Converters;
using Unite.Genome.Feed.Web.Services.Indexing;

namespace Unite.Genome.Feed.Web.Handlers.Annotation;

public class MutationsAnnotationHandler
{
    private readonly ISqlOptions _sqlOptions;
    private readonly IEnsemblOptions _ensemblDataOptions;
    private readonly IEnsemblVepOptions _ensemblVepOptions;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;


    public MutationsAnnotationHandler(
        ISqlOptions sqlOptions,
        IEnsemblOptions ensemblDataOptions,
        IEnsemblVepOptions ensemblVepOptions,
        TasksProcessingService taskProcessingService,
        ILogger<MutationsAnnotationHandler> logger)
    {
        _sqlOptions = sqlOptions;
        _ensemblDataOptions = ensemblDataOptions;
        _ensemblVepOptions = ensemblVepOptions;
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

        _taskProcessingService.Process(AnnotationTaskType.SSM, bucketSize, (tasks) =>
        {
            if (_taskProcessingService.HasSubmissionTasks())
            {
                return false;
            }

            _logger.LogInformation($"Annotating {tasks.Length} mutations");

            stopwatch.Restart();

            ProcessAnnotationTasks(tasks);

            stopwatch.Stop();

            _logger.LogInformation($"Annotation of {tasks.Length} mutations completed in {Math.Round(stopwatch.Elapsed.TotalSeconds, 2)}s");

            return true;
        });
    }

    private void ProcessAnnotationTasks(Unite.Data.Entities.Tasks.Task[] tasks)
    {
        // Database context needs to be disposed, otherwise write speed degrades.
        using var dbContext = new DomainDbContext(_sqlOptions);

        var annotationService = new MutationsAnnotationService(dbContext, _ensemblDataOptions, _ensemblVepOptions);
        var variantRepository = new VariantRepository(dbContext);
        var affectedTranscriptRepository = new AffectedTranscriptRepository(dbContext, variantRepository);
        var consequencesDataWriter = new ConsequencesDataWriter<AffectedTranscript, Variant, VariantModel>(dbContext, variantRepository, affectedTranscriptRepository);
        var indexingTaskService = new MutationIndexingTaskService(dbContext);

        var variants = tasks.Select(task => long.Parse(task.Target)).ToArray();
        var annotations = annotationService.Annotate(variants);
        var consequences = ConsequencesDataConverter.Convert(annotations);

        consequencesDataWriter.SaveData(consequences, out var audit);
        indexingTaskService.PopulateTasks(audit.Variants);

        _logger.LogInformation(audit.ToString());
    }
}
