using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class GenesIndexingHandler : IndexingHandler<GeneIndex, GenesIndexingCache>
{
    private readonly GenesIndexingOptions _options;

    public GenesIndexingHandler(GenesIndexingOptions options,
        TasksProcessingService taskProcessingService,
        IIndexService<GeneIndex> indexingService,
        GeneIndexEntityBuilder indexEntityBuilder,
        IDbContextFactory<DomainDbContext> dbContextFactory,
        ILogger<GenesIndexingHandler> logger) : base(taskProcessingService, indexingService, indexEntityBuilder, dbContextFactory, logger)
    {
        _options = options;
    }

    protected override int BucketSize => _options.BucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.Gene;
    protected override string IndexEntityKind => "Gene";
    protected override string GetIndexEntityKey(GeneIndex entity)
    {
        return entity.Id.ToString();
    }
}
