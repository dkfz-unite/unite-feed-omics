using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public abstract class IndexingHandler<TIndexingCache> : Handler, IIndexingHandler 
    where TIndexingCache : IndexingCache
{
    private readonly IDbContextFactory<DomainDbContext> _dbContextFactory;

    protected IndexingHandler(TasksProcessingService taskProcessingService,
        IDbContextFactory<DomainDbContext> dbContextFactory,
        ILogger logger)
    {
        TaskProcessingService = taskProcessingService;
        Logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    protected TasksProcessingService TaskProcessingService { get; }
    protected ILogger Logger { get; }

    protected abstract IIndexer<TIndexingCache>[] Indexers { get; }
    protected abstract int BucketSize { get; }
    protected abstract IndexingTaskType IndexingTaskType { get; }
    
    public Task Prepare()
    {
        List<Task> indexingJobs = [];
        foreach (var indexer in Indexers)
        {
            indexingJobs.Add(indexer.PrepareIndex());
        }
        return Task.WhenAll(indexingJobs.ToArray());
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
        
        TaskProcessingService.Process(IndexingTaskType, BucketSize, tasks =>
        {
            using var indexingCache = IndexingCache.Create<TIndexingCache>(_dbContextFactory, tasks.Select(task => int.Parse(task.Target)).ToArray());

            List<Task> indexingJobs = [];
            foreach (var indexer in Indexers)
            {
                indexingJobs.Add(indexer.BuildIndex(tasks, indexingCache));
            }
            Task.WaitAll(indexingJobs.ToArray());

            return true;
        });
        
        stopwatch.Stop();
        Logger.LogInformation("Indexing Task of type {indexingTaskType} is completed in {time}s", IndexingTaskType, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));
        return Task.CompletedTask;
    }
}
