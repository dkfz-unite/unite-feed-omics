using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

public class SmIndexer: Indexer<SmIndex, VariantIndexingCache<Variant, VariantEntry>>
{
    public SmIndexer(IIndexService<SmIndex> indexingService, 
        SmIndexEntityBuilder indexEntityBuilder, 
        ILogger logger) : base(indexingService, indexEntityBuilder, logger)
    {
    }

    public override string IndexEntityKind => "SM";
    
    protected override string GetIndexEntityKey(SmIndex entity)
    {
        return entity.Id.ToString();
    }
}