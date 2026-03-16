using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Indices.Services;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SmsIndexingHandler : IndexingHandler<VariantIndexingCache<Variant, VariantEntry>>
{
    private readonly VariantsIndexingOptions _options;
    private readonly SmIndexer _smIndexer;

    public SmsIndexingHandler(VariantsIndexingOptions options,
        TasksProcessingService taskProcessingService,
        IDbContextFactory<DomainDbContext> dbContextFactory,
        ILogger<SmsIndexingHandler> logger, 
        SmIndexer smIndexer) : base(taskProcessingService, dbContextFactory, logger)
    {
        _options = options;
        _smIndexer = smIndexer;
    }

    protected override IIndexer<VariantIndexingCache<Variant, VariantEntry>>[] Indexers =>
    [
        _smIndexer
    ];
    
    protected override int BucketSize => _options.SmBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.SM;
}
