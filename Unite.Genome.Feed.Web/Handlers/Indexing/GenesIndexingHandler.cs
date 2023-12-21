using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;

namespace Unite.Genome.Feed.Web.Handlers.Indexing;

public class GenesIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly GeneIndexCreationService _indexCreationService;
    private readonly IIndexService<GeneIndex> _indexingService;
    private readonly ILogger _logger;


    public GenesIndexingHandler(
        TasksProcessingService taskProcessingService,
        GeneIndexCreationService indexCreationService,
        IIndexService<GeneIndex> indexingService,
        ILogger<GenesIndexingHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _indexCreationService = indexCreationService;
        _indexingService = indexingService;
        _logger = logger;
    }


    public void Prepare()
    {
        _indexingService.UpdateIndex().GetAwaiter().GetResult();
    }

    public void Handle(int bucketSize)
    {
        ProcessIndexingTasks(bucketSize);
    }


    private void ProcessIndexingTasks(int bucketSize)
    {
        var stopwatch = new Stopwatch();
        
        _taskProcessingService.Process(IndexingTaskType.Gene, bucketSize, (tasks) =>
        {
            if (_taskProcessingService.HasSubmissionTasks() || _taskProcessingService.HasAnnotationTasks())
            {
                return false;
            }

            _logger.LogInformation("Indexing {number} genes", tasks.Length);

            stopwatch.Restart();

            var grouped = tasks.DistinctBy(task => task.Target);

            var indices = grouped.Select(task =>
            {
                var id = int.Parse(task.Target);

                var index = _indexCreationService.CreateIndex(id);

                return index;

            }).ToArray();

            _indexingService.AddRange(indices).GetAwaiter().GetResult();

            stopwatch.Stop();

            _logger.LogInformation("Indexing of {number} genes completed in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
