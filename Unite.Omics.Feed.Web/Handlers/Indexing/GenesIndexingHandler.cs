using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Indices.Services;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class GenesIndexingHandler: IndexingHandler<GenesIndexingCache>
{
    private readonly GeneIndexer _geneIndexer;
    private readonly GeneExpressionIndexer _geneExpressionIndexer;
    private readonly GenesIndexingOptions _options;
    
    public GenesIndexingHandler(TasksProcessingService taskProcessingService, 
        IDbContextFactory<DomainDbContext> dbContextFactory, 
        ILogger logger, 
        GeneIndexer geneIndexer, 
        GeneExpressionIndexer geneExpressionIndexer,
        GenesIndexingOptions options) : base(taskProcessingService, dbContextFactory, logger)
    {
        _geneIndexer = geneIndexer;
        _geneExpressionIndexer = geneExpressionIndexer;
        _options = options;
    }

    protected override IIndexer<GenesIndexingCache>[] Indexers =>
    [
        _geneIndexer, 
        _geneExpressionIndexer
    ];
    protected override int BucketSize => _options.BucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.Gene;
}
