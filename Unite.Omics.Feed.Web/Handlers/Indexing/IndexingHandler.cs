using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Indices.Context;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public abstract class IndexingHandler<TIndexEntity, TIndexingCache>(
    TasksProcessingService taskProcessingService,
    IIndexService<TIndexEntity> indexingService,
    IndexEntityBuilder<TIndexEntity, TIndexingCache> indexEntityBuilder,
    IDbContextFactory<DomainDbContext> dbContextFactory,
    ILogger logger
    ) : Handler, IIndexingHandler 
    where TIndexEntity : class
    where TIndexingCache : IndexingCache
{
    protected TasksProcessingService TaskProcessingService => taskProcessingService;
    protected IIndexService<TIndexEntity> IndexingService => indexingService;
    protected IndexEntityBuilder<TIndexEntity, TIndexingCache> IndexEntityBuilder => indexEntityBuilder;
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
            
            using var indexingCache = IndexingCache.Create<TIndexingCache>(dbContextFactory, tasks.Select(task => int.Parse(task.Target)).ToArray());
            
            var entitiesToDelete = new List<string>();
            var entitiesToCreate = new List<TIndexEntity>();

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                var indexEntity = IndexEntityBuilder.Create(id, indexingCache);

                if (indexEntity == null)
                    entitiesToDelete.Add($"{id}");
                else
                    entitiesToCreate.Add(indexEntity);

            });

            if (entitiesToDelete.Any())
                await IndexingService.DeleteRange(entitiesToDelete);

            if (entitiesToCreate.Any())
            {
                try
                {
                    await IndexingService.AddRange(entitiesToCreate);
                }
                catch
                {
                    foreach (var index in entitiesToCreate)
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

            stopwatch.Stop();

            Logger.LogInformation("Indexed {number} {entityKind} in {time}s", tasks.Length, IndexEntityKind, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
