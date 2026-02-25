using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class GenesIndexingHandler(
    GenesIndexingOptions options,
    TasksProcessingService taskProcessingService,
    GenesIndexingCache indexingCache,
    IIndexService<GeneIndex> indexingService,
    ILogger<GenesIndexingHandler> logger)
    : IndexingHandler
{
    public override async Task Prepare()
    {
        await indexingService.UpdateIndex();
    }

    public override async Task Handle()
    {
        await ProcessIndexingTasks(options.BucketSize);
    }
    
    private async Task ProcessIndexingTasks(int bucketSize)
    {
        if (taskProcessingService.HasTasks(WorkerType.Submission) || taskProcessingService.HasTasks(WorkerType.Annotation))
            return;

        var stopwatch = new Stopwatch();
        
        await taskProcessingService.Process(IndexingTaskType.Gene, bucketSize, async (tasks) =>
        {
            stopwatch.Restart();

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
                            logger.LogError(e, "Failed to index gene {id}", index.Id);
                        }
                    }
                }
            }

            indexingCache.Clear();

            stopwatch.Stop();

            logger.LogInformation("Indexed {number} genes in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
