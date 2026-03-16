using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

public class SvIndexer: Indexer<SvIndex, VariantIndexingCache<Variant, VariantEntry>>
{
    public SvIndexer(IIndexService<SvIndex> indexingService, 
        SvIndexEntityBuilder indexEntityBuilder, 
        ILogger logger) : base(indexingService, indexEntityBuilder, logger)
    {
    }

    public override string IndexEntityKind => "SV";
    protected override string GetIndexEntityKey(SvIndex entity)
    {
        return entity.Id.ToString();
    }
}