using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Genome.Variants.SSM;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Genome.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;

namespace Unite.Genome.Feed.Web.Handlers.Indexing;

public class SsmsIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly VariantIndexCreationService<Variant, VariantEntry> _indexCreationService;
    private readonly IIndexService<VariantIndex> _indexingService;
    private readonly ILogger _logger;


    public SsmsIndexingHandler(
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

        _taskProcessingService.Process(IndexingTaskType.SSM, bucketSize, (tasks) =>
        {
            if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            {
                return false;
            }

            _logger.LogInformation("Indexing {number} SSMs", tasks.Length);

            stopwatch.Restart();

            var indicesToRemove = new List<string>();
            var indicesToCreate = new List<VariantIndex>();

            tasks.ForEach(task =>
            {
                var id = long.Parse(task.Target);

                var index = _indexCreationService.CreateIndex(id);

                if (index == null)
                    indicesToRemove.Add($"SSM{id}");
                else
                    indicesToCreate.Add(index);
            });

            _indexingService.DeleteRange(indicesToRemove).GetAwaiter().GetResult();
            _indexingService.AddRange(indicesToCreate).GetAwaiter().GetResult();

            stopwatch.Stop();

            _logger.LogInformation("Indexing of {number} SSMs completed in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
