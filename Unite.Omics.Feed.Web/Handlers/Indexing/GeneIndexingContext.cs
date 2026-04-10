using Unite.Indices.Entities.Genes;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class GeneIndexingContext : IndexingContext<GeneIndex> 
{
    public List<GeneExpressionIndex> GeneExpressionsToAdd = [];
}
