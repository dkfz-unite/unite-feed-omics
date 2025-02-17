using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Genome.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;

namespace Unite.Genome.Feed.Web.Handlers.Indexing;

public class CnvsIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly VariantIndexingCache<Variant, VariantEntry> _indexingCache;
    private readonly IIndexService<CnvIndex> _indexingService;
    private readonly ILogger _logger;


    public CnvsIndexingHandler(
        TasksProcessingService taskProcessingService,
        VariantIndexingCache<Variant, VariantEntry> indexingCache,
        IIndexService<CnvIndex> indexingService,
        ILogger<CnvsIndexingHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _indexingCache = indexingCache;
        _indexingService = indexingService;
        _logger = logger;
    }


    public async Task Prepare()
    {
        await _indexingService.UpdateIndex();
    }

    public async Task Handle(int bucketSize)
    {
        await ProcessIndexingTasks(bucketSize);
    }


    private async Task ProcessIndexingTasks(int bucketSize)
    {
        var stopwatch = new Stopwatch();

        await _taskProcessingService.Process(IndexingTaskType.CNV, bucketSize, async (tasks) =>
        {
            if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
                return false;

            stopwatch.Restart();

            _indexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<CnvIndex>();
            var indexCreator = new CnvIndexCreator(_indexingCache);

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
                await _indexingService.DeleteRange(indicesToDelete);
            
            if (indicesToCreate.Any())
                await _indexingService.AddRange(indicesToCreate);

            _indexingCache.Clear();

            stopwatch.Stop();

            _logger.LogInformation("Indexed {number} CNVs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
