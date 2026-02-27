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

public class SmsIndexingHandler(
    VariantsIndexingOptions options,
    TasksProcessingService taskProcessingService,
    IIndexService<SmIndex> indexingService,
    SmIndexEntityBuilder indexEntityBuilder,
    IDbContextFactory<DomainDbContext> dbContextFactory,
    ILogger<SmsIndexingHandler> logger)
    : IndexingHandler<SmIndex, VariantIndexingCache<Variant, VariantEntry>>(taskProcessingService, indexingService, indexEntityBuilder, dbContextFactory, logger)
{
    protected override int BucketSize => options.SmBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.SM;
    protected override string IndexEntityKind => "SM";
    protected override string GetIndexEntityKey(SmIndex entity)
    {
        return entity.Id.ToString();
    }
}
