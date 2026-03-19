namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public interface IIndexingHandler : IHandler
{
    Task Prepare();
}
