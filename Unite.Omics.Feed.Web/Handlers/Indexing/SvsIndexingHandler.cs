using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Indices.Services;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SvsIndexingHandler : IndexingHandler<VariantIndexingCache<Variant, VariantEntry>>
{
    private readonly VariantsIndexingOptions _options;
    private readonly SvIndexer _svIndexer;

    public SvsIndexingHandler(VariantsIndexingOptions options,
        TasksProcessingService taskProcessingService,
        IDbContextFactory<DomainDbContext> dbContextFactory,
        ILogger<SvsIndexingHandler> logger, SvIndexer svIndexer) : base(taskProcessingService, dbContextFactory, logger)
    {
        _options = options;
        _svIndexer = svIndexer;
    }

    protected override IIndexer<VariantIndexingCache<Variant, VariantEntry>>[] Indexers =>
    [
        _svIndexer
    ];
    
    protected override int BucketSize => _options.SvBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.SV;
}
