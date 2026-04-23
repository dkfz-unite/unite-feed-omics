namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class IndexingContext<TIndexEntity>
{
    public List<TIndexEntity> EntitiesToAdd = [];
    public List<string> EntitiesToDelete = [];
}
