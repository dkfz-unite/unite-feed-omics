using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class CnvsIndexingHandler : IndexingHandler<CnvIndex>
{
    private readonly VariantsIndexingOptions _options;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger<CnvsIndexingHandler> _logger;

    public CnvsIndexingHandler(VariantsIndexingOptions options,
        TasksProcessingService taskProcessingService,
        IIndexService<CnvIndex> indexingService,
        VariantIndexingCache<Variant, VariantEntry> indexingCache,
        IIndexCreator<CnvIndex> indexCreator,
        ILogger<CnvsIndexingHandler> logger) : base(indexingService, indexingCache, indexCreator)
    {
        _options = options;
        _taskProcessingService = taskProcessingService;
        _logger = logger;
    }

    protected override async Task ProcessIndexingTasks()
    {
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            return;
                
        var stopwatch = new Stopwatch();

        await _taskProcessingService.Process(IndexingTaskType.CNV, _options.CnvBucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            IndexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<CnvIndex>();

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

            _logger.LogInformation("Indexed {number} CNVs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
