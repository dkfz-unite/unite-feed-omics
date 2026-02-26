namespace Unite.Omics.Indices.Services;

public interface IIndexingCache
{
    void Load(int[] ids);
    void Clear();
}