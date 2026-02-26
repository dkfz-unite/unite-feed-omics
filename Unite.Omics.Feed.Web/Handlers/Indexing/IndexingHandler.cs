using Unite.Indices.Context;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public abstract class IndexingHandler<TIndexEntity>(
    IIndexService<TIndexEntity> indexingService,
    IIndexingCache indexingCache,
    IIndexCreator<TIndexEntity> indexCreator
    ) : Handler, IIndexingHandler 
    where TIndexEntity : class
{
    protected IIndexService<TIndexEntity> IndexingService => indexingService;
    protected IIndexCreator<TIndexEntity> IndexCreator => indexCreator;
    protected IIndexingCache IndexingCache => indexingCache;
    
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
