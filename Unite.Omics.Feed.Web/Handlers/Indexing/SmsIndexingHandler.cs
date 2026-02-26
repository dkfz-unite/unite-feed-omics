using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SmsIndexingHandler : IndexingHandler<SmIndex>
{
    private readonly ILogger _logger;
    private readonly VariantsIndexingOptions _options;
    private readonly TasksProcessingService _taskProcessingService;

    public SmsIndexingHandler(VariantsIndexingOptions options,
        TasksProcessingService taskProcessingService,
        IIndexService<SmIndex> indexingService,
        VariantIndexingCache<Variant, VariantEntry> indexingCache,
        IIndexCreator<SmIndex> indexCreator,
        ILogger<SmsIndexingHandler> logger) : base(indexingService, indexingCache, indexCreator)
    {
        _options = options;
        _taskProcessingService = taskProcessingService;
        _logger = logger;
    }
    
    public override async Task Handle()
    {
        await ProcessIndexingTasks(_options.SmBucketSize);
    }

    private async Task ProcessIndexingTasks(int bucketSize)
    {
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            return;

        var stopwatch = new Stopwatch();

        await _taskProcessingService.Process(IndexingTaskType.SM, bucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            IndexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());
            
            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<SmIndex>();

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                var index = IndexCreator.Create(id);

                if (index == null)
                    indicesToDelete.Add($"{id}");
                else
                    indicesToCreate.Add(index);
            });

            if (indicesToDelete.Any())
                await IndexingService.DeleteRange(indicesToDelete);

            if (indicesToCreate.Any())
                await IndexingService.AddRange(indicesToCreate);

            IndexingCache.Clear();

            stopwatch.Stop();

            _logger.LogInformation("Indexed {number} SMs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
