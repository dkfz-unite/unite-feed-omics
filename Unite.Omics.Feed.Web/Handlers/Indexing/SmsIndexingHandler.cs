using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SmsIndexingHandler : IndexingHandler<SmIndex, VariantIndexingCache<Variant, VariantEntry>>
{
    private readonly VariantsIndexingOptions _options;

    public SmsIndexingHandler(VariantsIndexingOptions options,
        TasksProcessingService taskProcessingService,
        IIndexService<SmIndex> indexingService,
        SmIndexEntityBuilder indexEntityBuilder,
        IDbContextFactory<DomainDbContext> dbContextFactory,
        ILogger<SmsIndexingHandler> logger) : base(taskProcessingService, indexingService, indexEntityBuilder, dbContextFactory, logger)
    {
        _options = options;
    }

    protected override int BucketSize => _options.SmBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.SM;
    protected override string IndexEntityKind => "SM";
    protected override string GetIndexEntityKey(SmIndex entity)
    {
        return entity.Id.ToString();
    }
}
