using Unite.Indices.Entities.Proteins;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class ProteinIndexingContext : IndexingContext<ProteinIndex> 
{
    public List<ProteinExpressionIndex> ProteinExpressionsToAdd = [];
}
