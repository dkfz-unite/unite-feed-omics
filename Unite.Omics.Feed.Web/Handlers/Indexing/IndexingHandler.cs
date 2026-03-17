using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Indices.Context;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class IndexingContext<TIndexEntity>
{
    public List<TIndexEntity> EntitiesToAdd { get; } = [];
    public List<string> EntitiesToDelete { get; } = [];
}

public abstract class IndexingHandler<TIndexEntity, TIndexingCache, TIndexEntityBuilder, TIndexingContext> : Handler, IIndexingHandler
    where TIndexEntity : class
    where TIndexingCache : IndexingCache
    where TIndexingContext : IndexingContext<TIndexEntity>, new()
    where TIndexEntityBuilder : IndexEntityBuilder<TIndexEntity, TIndexingCache>
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;

    protected IndexingHandler(TasksProcessingService taskProcessingService,
        IDbContextFactory<DomainDbContext> dbContextFactory,
        ILogger logger, 
        IIndexService<TIndexEntity> indexingService, 
        TIndexEntityBuilder indexEntityBuilder)
    {
        TaskProcessingService = taskProcessingService;
        Logger = logger;
        IndexingService = indexingService;
        IndexEntityBuilder = indexEntityBuilder;
        _dbContextFactory = dbContextFactory;
    }

    private TasksProcessingService TaskProcessingService { get; }
    
    protected ILogger Logger { get; }
    
    protected IIndexService<TIndexEntity> IndexingService { get; }

    protected TIndexEntityBuilder IndexEntityBuilder { get; }

    protected abstract int BucketSize { get; }
    
    protected abstract IndexingTaskType IndexingTaskType { get; }
    
    protected abstract string IndexEntityKind { get; }

    public virtual async Task Prepare()
    {
        await IndexingService.CreateIndex();
    }

    public override async Task Handle()
    {
        await ProcessIndexingTasks();
    }

    private Task ProcessIndexingTasks()
    {
        if (TaskProcessingService.HasTasks(WorkerType.Submission) || TaskProcessingService.HasTasks(WorkerType.Annotation))
            return Task.CompletedTask;

        var stopwatch = Stopwatch.StartNew();
        
        TaskProcessingService.Process(IndexingTaskType, BucketSize, async tasks =>
        {
            var stopwatchBatch = Stopwatch.StartNew();
            using var indexingCache = IndexingCache.Create<TIndexingCache>(_dbContextFactory, tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indexingContext = new TIndexingContext();

            tasks.ForEach(async void (task) =>
            {
                var id = int.Parse(task.Target);
                await BuildIndexEntity(id, indexingCache, indexingContext);
            });

            await DeleteIndexEntities(indexingContext);
            await CreateIndexEntities(indexingContext);

            stopwatchBatch.Stop();
            Logger.LogInformation("Indexed {number} {entityKind} in {time}s", tasks.Length, IndexEntityKind, Math.Round(stopwatchBatch.Elapsed.TotalSeconds, 2));
            
            return true;
        });
        
        stopwatch.Stop();
        Logger.LogInformation("Indexing Task of type {indexingTaskType} is completed in {time}s", IndexingTaskType, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));
        return Task.CompletedTask;
    }

    protected virtual Task BuildIndexEntity(int id, TIndexingCache indexingCache, TIndexingContext indexingContext)
    {
        var indexEntities = IndexEntityBuilder.Create(id, indexingCache);

        if (indexEntities == null || indexEntities.Length == 0)
            indexingContext.EntitiesToDelete.Add($"{id}");
        else
            indexingContext.EntitiesToAdd.AddRange(indexEntities);
        
        return Task.CompletedTask;
    }
    
    protected virtual async Task DeleteIndexEntities(TIndexingContext indexingContext)
    {
        if(indexingContext.EntitiesToDelete.Any())
            await IndexingService.DeleteRange(indexingContext.EntitiesToDelete);
    }

    protected virtual async Task CreateIndexEntities(TIndexingContext indexingContext)
    {
        if (indexingContext.EntitiesToAdd.Any())
            await IndexingService.AddRange(indexingContext.EntitiesToAdd);
    }
}