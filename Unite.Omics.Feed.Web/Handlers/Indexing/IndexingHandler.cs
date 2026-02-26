using Unite.Indices.Context;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public abstract class IndexingHandler<TIndexEntity>(
    IIndexService<TIndexEntity> indexingService
    ) : Handler, IIndexingHandler 
    where TIndexEntity : class
{
    protected IIndexService<TIndexEntity> IndexingService => indexingService;
    
    public async Task Prepare()
    {
        await IndexingService.UpdateIndex();
    }
}