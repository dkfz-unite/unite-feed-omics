using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Feed.Web.Configuration.Options;
using Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class CnvsIndexingHandler : IndexingHandler<VariantIndexingCache<Variant, VariantEntry>>
{
    private readonly VariantsIndexingOptions _options;
    private readonly CnvIndexer _cnvIndexer;

    public CnvsIndexingHandler(VariantsIndexingOptions options,
        TasksProcessingService taskProcessingService,
        IDbContextFactory<DomainDbContext> dbContextFactory,
        CnvIndexer cnvIndexer,
        ILogger<CnvsIndexingHandler> logger) : base(taskProcessingService, dbContextFactory, logger)
    {
        _options = options;
        _cnvIndexer = cnvIndexer;
    }

    protected override IIndexer<VariantIndexingCache<Variant, VariantEntry>>[] Indexers =>
    [
        _cnvIndexer
    ];
    
    protected override int BucketSize => _options.CnvBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.CNV;
}
