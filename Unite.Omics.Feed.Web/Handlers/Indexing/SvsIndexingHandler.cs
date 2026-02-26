using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SvsIndexingHandler(
    VariantsIndexingOptions options,
    TasksProcessingService taskProcessingService,
    VariantIndexingCache<Variant, VariantEntry> indexingCache,
    IIndexService<SvIndex> indexingService,
    IIndexCreator<SvIndex> indexCreator,
    ILogger<SvsIndexingHandler> logger)
    : IndexingHandler<SvIndex>(taskProcessingService, indexingService, indexingCache, indexCreator, logger)
{
    protected override int BucketSize => options.SvBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.SV;
    protected override string IndexEntityKind => "SV";
    protected override string GetIndexEntityKey(SvIndex entity)
    {
        return entity.Id.ToString();
    }
}
