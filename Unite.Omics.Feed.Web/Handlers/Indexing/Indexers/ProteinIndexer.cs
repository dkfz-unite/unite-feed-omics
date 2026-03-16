using Unite.Indices.Context;
using Unite.Indices.Entities.Proteins;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

public class ProteinIndexer: Indexer<ProteinIndex, ProteinsIndexingCache>
{
    public ProteinIndexer(IIndexService<ProteinIndex> indexingService, 
        ProteinIndexEntityBuilder indexEntityBuilder, 
        ILogger logger) : base(indexingService, indexEntityBuilder, logger)
    {
    }

    public override string IndexEntityKind => "Proteins";
    protected override string GetIndexEntityKey(ProteinIndex entity)
    {
        return entity.Id.ToString();
    }
    
    public override async Task PrepareIndex()
    {
        await IndexingService.CreateIndex();
    }
}