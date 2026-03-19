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


    public virtual Task Prepare()
    {
        return _indexingService.CreateIndex();
    }

    public override async Task Handle()
    {
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            return;
        
        await _taskProcessingService.Process(IndexingTaskType, BucketSize, ProcessChunk);
        
        return;
    }

    protected virtual async Task<bool> ProcessChunk(Unite.Data.Entities.Tasks.Task[] tasks)
    {
        var stopwatch = Stopwatch.StartNew();

        var ids = tasks.Select(task => int.Parse(task.Target)).ToArray();
        var cache = IndexingCache.Create<TIndexingCache>(_dbContextFactory, ids);
        var ontext = new TIndexingContext();

        foreach (var task in tasks)
        {
            var id = int.Parse(task.Target);
            await BuildIndexEntity(id, cache, ontext);
        }

        await DeleteIndexEntities(ontext);
        await CreateIndexEntities(ontext);

        stopwatch.Stop();
        
        _logger.LogInformation("Indexed {number} {entityKind} in {time}s", tasks.Length, IndexEntityKind, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));
        
        return true;
    }

    protected virtual Task BuildIndexEntity(int id, TIndexingCache indexingCache, TIndexingContext indexingContext)
    {
        var indexEntities = _indexEntityBuilder.Create(id, indexingCache);

        if (indexEntities.IsEmpty())
            indexingContext.EntitiesToDelete.Add($"{id}");
        else
            indexingContext.EntitiesToAdd.AddRange(indexEntities);
        
        return Task.CompletedTask;
    }
    
    protected virtual Task DeleteIndexEntities(TIndexingContext indexingContext)
    {
        if(indexingContext.EntitiesToDelete.Any())
            return _indexingService.DeleteRange(indexingContext.EntitiesToDelete);

        return Task.CompletedTask;
    }

    protected virtual Task CreateIndexEntities(TIndexingContext indexingContext)
    {
        if (indexingContext.EntitiesToAdd.Any())
            return _indexingService.AddRange(indexingContext.EntitiesToAdd);
        
        return Task.CompletedTask;
    }
}
