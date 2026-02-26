using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Context;
using Unite.Indices.Entities.CnvProfiles;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class CnvProfilesIndexingHandler : IndexingHandler<CnvProfileIndex>
{
    private readonly TasksProcessingService _taskProcessingService;

    public CnvProfilesIndexingHandler(TasksProcessingService taskProcessingService,
        CnvProfileIndexingCache indexingCache,
        IIndexService<CnvProfileIndex> indexingService,
        ILogger<GenesIndexingHandler> logger) : base(indexingService)
    {
        _taskProcessingService = taskProcessingService;
    }

    public override async Task Handle()
    {
        //TODO: move bucket size to Options
        await ProcessIndexingTasks(100);
    }

    private async Task ProcessIndexingTasks(int bucketSize)
    {
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            return;

        var stopwatch = new Stopwatch();
        
        await _taskProcessingService.Process(IndexingTaskType.Gene, bucketSize, async (tasks) =>
        {
            /*stopwatch.Restart();

            indexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<GeneIndex>();
            var indexCreator = new GeneIndexCreator(indexingCache);

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
                await indexingService.DeleteRange(indicesToDelete);

            if (indicesToCreate.Any())
            {
                try
                {
                    await indexingService.AddRange(indicesToCreate);
                }
                catch
                {
                    foreach (var index in indicesToCreate)
                    {
                        try
                        {
                            await indexingService.Add(index);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Failed to index gene {id}", index.Id);
                        }
                    }
                }
            }

            indexingCache.Clear();

            stopwatch.Stop();

            _logger.LogInformation("Indexed {number} genes in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));*/

            return true;
        });
    }
}