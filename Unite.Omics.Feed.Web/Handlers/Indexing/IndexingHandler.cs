using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Indices.Context;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public abstract class IndexingHandler<TIndexEntity, TIndexingCache, TIndexEntityBuilder, TIndexingContext> : IHandler
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


    public void Prepare()
    {
        _indexingService.CreateIndex().Wait();
    }

    public void Handle()
    {
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            return;
        
        _taskProcessingService.Process(IndexingTaskType, BucketSize, ProcessChunk);        
    }

    protected virtual bool ProcessChunk(Unite.Data.Entities.Tasks.Task[] tasks)
    {
        var stopwatch = Stopwatch.StartNew();

        var ids = tasks.Select(task => int.Parse(task.Target)).ToArray();
        var cache = IndexingCache.Create<TIndexingCache>(_dbContextFactory, ids);
        var context = new TIndexingContext();

        foreach (var task in tasks)
        {
            var id = int.Parse(task.Target);
            BuildIndexEntity(id, cache, context);
        }

        DeleteIndexEntities(context);
        CreateIndexEntities(context);

        stopwatch.Stop();
        
        _logger.LogInformation("Indexed {number} entities in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));
        
        return true;
    }

    protected virtual void BuildIndexEntity(int id, TIndexingCache indexingCache, TIndexingContext indexingContext)
    {
        var indexEntities = _indexEntityBuilder.Create(id, indexingCache);

        if (indexEntities.IsEmpty())
            indexingContext.EntitiesToDelete.Add($"{id}");
        else
            indexingContext.EntitiesToAdd.AddRange(indexEntities);
            
    }
    
    protected virtual void DeleteIndexEntities(TIndexingContext indexingContext)
    {
        if(indexingContext.EntitiesToDelete.Any())
            _indexingService.DeleteRange(indexingContext.EntitiesToDelete).Wait();
    }

    protected virtual void CreateIndexEntities(TIndexingContext indexingContext)
    {
        if (indexingContext.EntitiesToAdd.Any())
            _indexingService.AddRange(indexingContext.EntitiesToAdd).Wait();
    }
}
