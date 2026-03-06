using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SvsIndexingHandler : IndexingHandler<SvIndex, VariantIndexingCache<Variant, VariantEntry>>
{
    private readonly VariantsIndexingOptions _options;

    public SvsIndexingHandler(VariantsIndexingOptions options,
        TasksProcessingService taskProcessingService,
        IIndexService<SvIndex> indexingService,
        SvIndexEntityBuilder indexEntityBuilder,
        IDbContextFactory<DomainDbContext> dbContextFactory,
        ILogger<SvsIndexingHandler> logger) : base(taskProcessingService, indexingService, indexEntityBuilder, dbContextFactory, logger)
    {
        _options = options;
    }

    protected override int BucketSize => _options.SvBucketSize;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.SV;
    protected override string IndexEntityKind => "SV";
    protected override string GetIndexEntityKey(SvIndex entity)
    {
        return entity.Id.ToString();
    }
}
