using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

public class CnvIndexer: Indexer<CnvIndex, VariantIndexingCache<Variant, VariantEntry>>
{
    public CnvIndexer(IIndexService<CnvIndex> indexingService, 
        CnvIndexEntityBuilder indexEntityBuilder, 
        ILogger logger) : base(indexingService, indexEntityBuilder, logger)
    {
    }

    public override string IndexEntityKind => "CNV";
    protected override string GetIndexEntityKey(CnvIndex entity)
    {
        return entity.Id.ToString();
    }
}