using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class CnvsIndexingHandler(
    VariantsIndexingOptions options,
    TasksProcessingService taskProcessingService,
    IIndexService<CnvIndex> indexingService,
    CnvIndexEntityBuilder indexEntityBuilder,
    IDbContextFactory<DomainDbContext> dbContextFactory,
    ILogger<CnvsIndexingHandler> logger)
    : IndexingHandler<CnvIndex, VariantIndexingCache<Variant, VariantEntry>>(taskProcessingService, indexingService, indexEntityBuilder, dbContextFactory, logger)
{
    protected override int BucketSize => options.CnvBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.CNV;
    protected override string IndexEntityKind => "CNV";
    protected override string GetIndexEntityKey(CnvIndex entity)
    {
        return entity.Id.ToString();
    }
}
