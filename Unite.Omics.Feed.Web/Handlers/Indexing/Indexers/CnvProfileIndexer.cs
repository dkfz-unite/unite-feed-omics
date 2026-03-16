using Unite.Indices.Context;
using Unite.Indices.Entities.CnvProfiles;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

public class CnvProfileIndexer: Indexer<CnvProfileIndex, CnvProfileIndexingCache>
{
    public CnvProfileIndexer(IIndexService<CnvProfileIndex> indexingService, 
        CnvProfileIndexEntityBuilder indexEntityBuilder, 
        ILogger logger) : base(indexingService, indexEntityBuilder, logger)
    {
    }

    public override string IndexEntityKind => "CnvProfile";
    
    protected override string GetIndexEntityKey(CnvProfileIndex entity)
    {
        return entity.Id.ToString();
    }
}