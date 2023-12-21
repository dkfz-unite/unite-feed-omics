using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Genome.Variants.CNV;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;

namespace Unite.Genome.Feed.Web.Handlers.Indexing;

public class CnvsIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly VariantIndexCreationService<Variant, VariantEntry> _indexCreationService;
    private readonly IIndexService<VariantIndex> _indexingService;
    private readonly ILogger _logger;


    public CnvsIndexingHandler(
        TasksProcessingService taskProcessingService,
        VariantIndexCreationService<Variant, VariantEntry> indexCreationService,
        IIndexService<VariantIndex> indexingService,
        ILogger<SsmsIndexingHandler> logger)
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

        _taskProcessingService.Process(IndexingTaskType.CNV, bucketSize, (tasks) =>
        {
            if (_taskProcessingService.HasSubmissionTasks() || _taskProcessingService.HasAnnotationTasks())
            {
                return false;
            }

            _logger.LogInformation("Indexing {number} CNVs", tasks.Length);

            stopwatch.Restart();

            var grouped = tasks.DistinctBy(task => task.Target);

            var indices = grouped.Select(task =>
            {
                var id = long.Parse(task.Target);

                var index = _indexCreationService.CreateIndex(id);

                return index;

            }).ToArray();

            _indexingService.AddRange(indices).GetAwaiter().GetResult();

            stopwatch.Stop();

            _logger.LogInformation("Indexing of {number} CNVs completed in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
