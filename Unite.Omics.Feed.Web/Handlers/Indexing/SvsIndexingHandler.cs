using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SvsIndexingHandler : IndexingHandler<SvIndex>
{
    private readonly ILogger _logger;
    private readonly VariantsIndexingOptions _options;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly VariantIndexingCache<Variant, VariantEntry> _indexingCache;

    public SvsIndexingHandler(VariantsIndexingOptions options,
        TasksProcessingService taskProcessingService,
        VariantIndexingCache<Variant, VariantEntry> indexingCache,
        IIndexService<SvIndex> indexingService,
        ILogger<SvsIndexingHandler> logger) : base(indexingService)
    {
        _options = options;
        _taskProcessingService = taskProcessingService;
        _indexingCache = indexingCache;
        _logger = logger;
    }
    
    public override async Task Handle()
    {
        await ProcessIndexingTasks(_options.SvBucketSize);
    }

    private async Task ProcessIndexingTasks(int bucketSize)
    {
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            return;

        var stopwatch = new Stopwatch();

        await _taskProcessingService.Process(IndexingTaskType.SV, bucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            _indexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<SvIndex>();
            var indexCreator = new SvIndexCreator(_indexingCache);

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                var index = indexCreator.CreateIndex(id);

                if (index == null)
                    indicesToDelete.Add($"{id}");
                else
                    indicesToCreate.Add(index);
            });

            if (indicesToDelete.Any())
                await IndexingService.DeleteRange(indicesToDelete);
            
            if (indicesToCreate.Any())
                await IndexingService.AddRange(indicesToCreate);

            _indexingCache.Clear();

            stopwatch.Stop();

            _logger.LogInformation("Indexed {number} SVs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
