using Unite.Indices.Context;
using Unite.Indices.Entities.Proteins;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

public class ProteinExpressionIndexer: Indexer<ProteinExpressionIndex, ProteinsIndexingCache>
{
    public ProteinExpressionIndexer(IIndexService<ProteinExpressionIndex> indexingService, 
        ProteinExpressionIndexEntityBuilder indexEntityBuilder, 
        ILogger logger) : base(indexingService, indexEntityBuilder, logger)
    {
    }

    public override string IndexEntityKind => "ProteinExpression";
    protected override string GetIndexEntityKey(ProteinExpressionIndex entity)
    {
        return entity.Id;
    }
    
    public override async Task PrepareIndex()
    {
        await IndexingService.CreateIndex();
    }
}