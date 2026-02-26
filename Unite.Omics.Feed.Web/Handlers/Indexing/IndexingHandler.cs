using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Indices.Context;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public abstract class IndexingHandler<TIndexEntity>(
    TasksProcessingService taskProcessingService,
    IIndexService<TIndexEntity> indexingService,
    IIndexingCache indexingCache,
    IIndexCreator<TIndexEntity> indexCreator,
    ILogger logger
    ) : Handler, IIndexingHandler 
    where TIndexEntity : class
{
    protected TasksProcessingService TaskProcessingService => taskProcessingService;
    protected IIndexService<TIndexEntity> IndexingService => indexingService;
    protected IIndexCreator<TIndexEntity> IndexCreator => indexCreator;
    protected IIndexingCache IndexingCache => indexingCache;
    protected ILogger Logger => logger;
    
    protected abstract int BucketSize { get; }
    protected abstract IndexingTaskType IndexingTaskType { get; }
    protected abstract string IndexEntityKind { get; }

    protected abstract string GetIndexEntityKey(TIndexEntity entity);
    
    public async Task Prepare()
    {
        await IndexingService.UpdateIndex();
    }
    
    public override async Task Handle()
    {
        await ProcessIndexingTasks();
    }
    private async Task ProcessIndexingTasks()
    {
        if (TaskProcessingService.HasTasks(WorkerType.Submission) || TaskProcessingService.HasTasks(WorkerType.Annotation))
            return;

        var stopwatch = new Stopwatch();
        
        await TaskProcessingService.Process(IndexingTaskType, BucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            IndexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<TIndexEntity>();

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
                            Logger.LogError(e, "Failed to index {entityKind} {id}", IndexEntityKind, GetIndexEntityKey(index));
                        }
                    }
                }
            }

            IndexingCache.Clear();

            stopwatch.Stop();

            Logger.LogInformation("Indexed {number} {entityKind} in {time}s", tasks.Length, IndexEntityKind, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
