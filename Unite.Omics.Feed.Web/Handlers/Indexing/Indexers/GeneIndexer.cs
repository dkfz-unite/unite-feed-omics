using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

public class GeneIndexer: Indexer<GeneIndex, GenesIndexingCache>
{
    public GeneIndexer(IIndexService<GeneIndex> indexingService, 
        GeneIndexEntityBuilder indexEntityBuilder, 
        ILogger logger) : base(indexingService, indexEntityBuilder, logger)
    {
    }

    public override string IndexEntityKind => "Genes";
    
    protected override string GetIndexEntityKey(GeneIndex entity)
    {
        return entity.Id.ToString();
    }
}