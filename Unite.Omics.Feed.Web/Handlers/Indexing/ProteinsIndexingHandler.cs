using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Indices.Context;
using Unite.Indices.Entities.Proteins;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class ProteinsIndexingHandler: IndexingHandler<ProteinsIndexingCache>
{
    private readonly ProteinIndexer _proteinIndexer;
    private readonly ProteinExpressionIndexer _proteinExpressionIndexer;
    private readonly ProteinsIndexingOptions _options;
    
    public ProteinsIndexingHandler(TasksProcessingService taskProcessingService, 
        IDbContextFactory<DomainDbContext> dbContextFactory, 
        ILogger logger, 
        ProteinIndexer proteinIndexer, 
        ProteinExpressionIndexer proteinExpressionIndexer, 
        ProteinsIndexingOptions options) : base(taskProcessingService, dbContextFactory, logger)
    {
        _proteinIndexer = proteinIndexer;
        _proteinExpressionIndexer = proteinExpressionIndexer;
        _options = options;
    }

    protected override IIndexer<ProteinsIndexingCache>[] Indexers =>
    [
        _proteinIndexer,
        _proteinExpressionIndexer
    ];
    
    protected override int BucketSize => _options.BucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.Protein;
}
