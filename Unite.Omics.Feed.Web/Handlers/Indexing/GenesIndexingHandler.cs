using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class GenesIndexingHandler(
    GenesIndexingOptions options,
    TasksProcessingService taskProcessingService,
    GenesIndexingCache indexingCache,
    IIndexService<GeneIndex> indexingService,
    IIndexCreator<GeneIndex> indexCreator,
    ILogger<GenesIndexingHandler> logger)
    : IndexingHandler<GeneIndex>(taskProcessingService, indexingService, indexingCache, indexCreator, logger)
{
    protected override int BucketSize => options.BucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.Gene;
    protected override string IndexEntityKind => "Gene";
    protected override string GetIndexEntityKey(GeneIndex entity)
    {
        return entity.Id.ToString();
    }
}
