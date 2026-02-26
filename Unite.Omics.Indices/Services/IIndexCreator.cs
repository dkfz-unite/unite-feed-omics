namespace Unite.Omics.Indices.Services;

public interface IIndexCreator<out TIndexEntity>
{
    TIndexEntity Create(int key);
}