using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public interface IIndexer<TIndexingCache>
    where TIndexingCache : IndexingCache
{
    protected string IndexEntityKind { get; }
    public Task PrepareIndex();
    Task BuildIndex(Unite.Data.Entities.Tasks.Task[] tasks, TIndexingCache indexingCache);
}