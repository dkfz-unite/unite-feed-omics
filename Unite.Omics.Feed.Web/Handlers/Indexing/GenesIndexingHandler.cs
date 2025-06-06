using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class GenesIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly GenesIndexingCache _indexingCache;
    private readonly IIndexService<GeneIndex> _indexingService;
    private readonly ILogger _logger;


    public GenesIndexingHandler(
        TasksProcessingService taskProcessingService,
        GenesIndexingCache indexingCache,
        IIndexService<GeneIndex> indexingService,
        ILogger<GenesIndexingHandler> logger)
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
        
        await _taskProcessingService.Process(IndexingTaskType.Gene, bucketSize, async (tasks) =>
        {
            if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
                return false;

            stopwatch.Restart();

            _indexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<GeneIndex>();
            var indexCreator = new GeneIndexCreator(_indexingCache);

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
            {
                try
                {
                    await _indexingService.AddRange(indicesToCreate);
                }
                catch
                {
                    foreach (var index in indicesToCreate)
                    {
                        try
                        {
                            await _indexingService.Add(index);
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
