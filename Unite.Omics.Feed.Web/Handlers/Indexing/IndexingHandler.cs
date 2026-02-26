using Unite.Data.Context.Services.Tasks;
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
    
    public async Task Prepare()
    {
        await IndexingService.UpdateIndex();
    }
    
    public override async Task Handle()
    {
        await ProcessIndexingTasks();
    }
    protected virtual Task ProcessIndexingTasks()
    {
        return Task.CompletedTask;
    }
}
