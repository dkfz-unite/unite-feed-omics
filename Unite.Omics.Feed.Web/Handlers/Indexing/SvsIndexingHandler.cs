using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Indices.Services;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SvsIndexingHandler: IndexingHandler<SvIndex, VariantIndexingCache<Variant, VariantEntry>, SvIndexEntityBuilder, IndexingContext<SvIndex>>
{
    protected override int BucketSize => _options.SvBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.SV;
    protected override string IndexEntityKind => "SV";

    private readonly VariantsIndexingOptions _options;
    
    public SvsIndexingHandler( 
        IDbContextFactory<DomainDbContext> dbContextFactory,
        TasksProcessingService taskProcessingService,
        SvIndexEntityBuilder indexEntityBuilder,
        IIndexService<SvIndex> indexingService,
        ILogger logger,
        VariantsIndexingOptions options) : base(dbContextFactory, taskProcessingService, indexEntityBuilder, indexingService, logger)
    {
        _options = options;
    }
}
