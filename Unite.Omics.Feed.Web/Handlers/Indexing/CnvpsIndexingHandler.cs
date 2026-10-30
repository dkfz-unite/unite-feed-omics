using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Context;
using Unite.Indices.Entities.CnvProfiles;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class CnvpsIndexingHandler: IndexingHandler<CnvProfileIndex, CnvpIndexingCache, CnvpIndexEntityBuilder, IndexingContext<CnvProfileIndex>>
{
    protected override int BucketSize => 100;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.CNVProfile;

    public CnvpsIndexingHandler(
        IDbContextFactory<DomainDbContext> dbContextFactory,
        TasksProcessingService taskProcessingService,
        CnvpIndexEntityBuilder indexEntityBuilder,
        IIndexService<CnvProfileIndex> indexingService,
        ILogger<CnvpsIndexingHandler> logger) : base(dbContextFactory, taskProcessingService, indexEntityBuilder, indexingService, logger)
    {
    }
}
