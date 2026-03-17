using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Indices.Services;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SmsIndexingHandler: IndexingHandler<SmIndex, VariantIndexingCache<Variant, VariantEntry>, SmIndexEntityBuilder, IndexingContext<SmIndex>>
{
    protected override int BucketSize => _options.SmBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.SM;
    protected override string IndexEntityKind => "SM";

    private readonly VariantsIndexingOptions _options;
    
    public SmsIndexingHandler( 
        IDbContextFactory<DomainDbContext> dbContextFactory,
        TasksProcessingService taskProcessingService,
        SmIndexEntityBuilder indexEntityBuilder,
        IIndexService<SmIndex> indexingService,
        ILogger logger,
        VariantsIndexingOptions options) : base(dbContextFactory, taskProcessingService, indexEntityBuilder, indexingService, logger)
    {
        _options = options;
    }
}
