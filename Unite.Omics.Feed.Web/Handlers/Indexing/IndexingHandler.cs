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
    protected readonly IDbContextFactory<DomainDbContext> _dbContextFactory;
    protected readonly TasksProcessingService _taskProcessingService;
    protected readonly TIndexEntityBuilder _indexEntityBuilder;
    protected readonly IIndexService<TIndexEntity> _indexingService;
    protected readonly ILogger _logger;

    protected abstract int BucketSize { get; }
    protected abstract IndexingTaskType IndexingTaskType { get; }
    protected abstract string IndexEntityKind { get; }


    protected IndexingHandler(
        IDbContextFactory<DomainDbContext> dbContextFactory,
        TasksProcessingService taskProcessingService,
        TIndexEntityBuilder indexEntityBuilder,
        IIndexService<TIndexEntity> indexingService,
        ILogger logger)
    {
        _dbContextFactory = dbContextFactory;
        _taskProcessingService = taskProcessingService;
        _indexEntityBuilder = indexEntityBuilder;
        _indexingService = indexingService;
        _logger = logger;
    }


    public virtual async Task Prepare()
    {
        await _indexingService.CreateIndex();
    }

    public override async Task Handle()
    {
        await ProcessIndexingTasks();
    }

    private Task ProcessIndexingTasks()
    {
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            return Task.CompletedTask;
        
        _taskProcessingService.Process(IndexingTaskType, BucketSize, async tasks =>
        {
            var stopwatch = Stopwatch.StartNew();
            using var indexingCache = IndexingCache.Create<TIndexingCache>(_dbContextFactory, tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indexingContext = new TIndexingContext();

            tasks.ForEach(async void (task) =>
            {
                var id = int.Parse(task.Target);
                await BuildIndexEntity(id, indexingCache, indexingContext);
            });

            await DeleteIndexEntities(indexingContext);
            await CreateIndexEntities(indexingContext);

            stopwatch.Stop();
            _logger.LogInformation("Indexed {number} {entityKind} in {time}s", tasks.Length, IndexEntityKind, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));
            
            return true;
        });
        
        return Task.CompletedTask;
    }

    protected virtual Task BuildIndexEntity(int id, TIndexingCache indexingCache, TIndexingContext indexingContext)
    {
        var indexEntities = _indexEntityBuilder.Create(id, indexingCache);

        if (indexEntities == null || indexEntities.Length == 0)
            indexingContext.EntitiesToDelete.Add($"{id}");
        else
            indexingContext.EntitiesToAdd.AddRange(indexEntities);
        
        return Task.CompletedTask;
    }
    
    protected virtual async Task DeleteIndexEntities(TIndexingContext indexingContext)
    {
        if(indexingContext.EntitiesToDelete.Any())
            await _indexingService.DeleteRange(indexingContext.EntitiesToDelete);
    }

    protected virtual async Task CreateIndexEntities(TIndexingContext indexingContext)
    {
        if (indexingContext.EntitiesToAdd.Any())
            await _indexingService.AddRange(indexingContext.EntitiesToAdd);
    }
}
