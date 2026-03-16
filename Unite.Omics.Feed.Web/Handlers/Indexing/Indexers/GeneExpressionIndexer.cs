using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

public class GeneExpressionIndexer: Indexer<GeneExpressionIndex, GenesIndexingCache>
{
    public GeneExpressionIndexer(IIndexService<GeneExpressionIndex> indexingService, 
        GeneExpressionIndexEntityBuilder indexEntityBuilder, 
        ILogger logger) : base(indexingService, indexEntityBuilder, logger)
    {
    }

    public override string IndexEntityKind => "Gene Expression";
    
    protected override string GetIndexEntityKey(GeneExpressionIndex entity)
    {
        return entity.Id;
    }

    protected override Task DeleteIndexEntities(IList<string> entities)
    {
        return Task.CompletedTask;
    }
}