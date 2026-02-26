namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public abstract class IndexingHandler : Handler, IIndexingHandler
{
    public abstract Task Prepare();
}