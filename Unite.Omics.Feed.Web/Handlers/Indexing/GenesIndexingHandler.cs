using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class GenesIndexingHandler : IndexingHandler<GeneIndex>
{
    private readonly GenesIndexingOptions _options;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly GenesIndexingCache _indexingCache;
    private readonly ILogger<GenesIndexingHandler> _logger;

    public GenesIndexingHandler(GenesIndexingOptions options,
        TasksProcessingService taskProcessingService,
        GenesIndexingCache indexingCache,
        IIndexService<GeneIndex> indexingService,
        IIndexCreator<GeneIndex> indexCreator,
        ILogger<GenesIndexingHandler> logger): base(indexingService, indexCreator)
    {
        _options = options;
        _taskProcessingService = taskProcessingService;
        _indexingCache = indexingCache;
        _logger = logger;
    }
    
    protected override async Task ProcessIndexingTasks()
    {
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            return;

        var stopwatch = new Stopwatch();
        
        await _taskProcessingService.Process(IndexingTaskType.Gene, _options.BucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            _indexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<GeneIndex>();

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
            {
                try
                {
                    await IndexingService.AddRange(indicesToCreate);
                }
                catch
                {
                    foreach (var index in indicesToCreate)
                    {
                        try
                        {
                            await IndexingService.Add(index);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Failed to index gene {id}", index.Id);
                        }
                    }
                }
            }

            _indexingCache.Clear();

            stopwatch.Stop();

            _logger.LogInformation("Indexed {number} genes in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
