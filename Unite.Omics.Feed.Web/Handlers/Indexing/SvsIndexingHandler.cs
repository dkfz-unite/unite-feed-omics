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
    private readonly VariantsIndexingOptions _options;
    
    public SvsIndexingHandler(TasksProcessingService taskProcessingService, 
        IDbContextFactory<DomainDbContext> dbContextFactory, 
        ILogger logger, 
        IIndexService<SvIndex> indexingService, 
        SvIndexEntityBuilder indexEntityBuilder, 
        VariantsIndexingOptions options) : base(taskProcessingService, dbContextFactory, logger, indexingService, indexEntityBuilder)
    {
        _options = options;
    }

    protected override int BucketSize => _options.SvBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.SV;
    protected override string IndexEntityKind => "SV";
}
