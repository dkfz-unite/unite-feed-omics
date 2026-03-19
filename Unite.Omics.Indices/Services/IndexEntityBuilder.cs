namespace Unite.Omics.Indices.Services;

public abstract class IndexEntityBuilder<TIndexEntity, TIndexingCache>
    where TIndexingCache: IndexingCache
{
    // TODO: Return empty collection (with no elements inside) instead of null or collection with nulls inside to eliminate the need for null checks.
    public abstract TIndexEntity[] Create(int key, TIndexingCache cache);
}
