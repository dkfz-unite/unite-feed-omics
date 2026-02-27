namespace Unite.Omics.Indices.Services;

public abstract class IndexEntityBuilder<TIndexEntity, TIndexingCache>
    where TIndexingCache: IndexingCache
{
    public abstract TIndexEntity Create(int key, TIndexingCache cache);
}