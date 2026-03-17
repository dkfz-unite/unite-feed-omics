using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Indices.Services;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class CnvsIndexingHandler: IndexingHandler<CnvIndex, VariantIndexingCache<Variant, VariantEntry>, CnvIndexEntityBuilder, IndexingContext<CnvIndex>>
{
    protected override int BucketSize => _options.CnvBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.CNV;
    protected override string IndexEntityKind => "CNV";

    private readonly VariantsIndexingOptions _options;
    
    public CnvsIndexingHandler( 
        IDbContextFactory<DomainDbContext> dbContextFactory,
        TasksProcessingService taskProcessingService,
        CnvIndexEntityBuilder indexEntityBuilder,
        IIndexService<CnvIndex> indexingService,
        VariantsIndexingOptions options,
        ILogger<CnvsIndexingHandler> logger) : base(dbContextFactory, taskProcessingService, indexEntityBuilder, indexingService, logger)
    {
        _options = options;
    }
}
