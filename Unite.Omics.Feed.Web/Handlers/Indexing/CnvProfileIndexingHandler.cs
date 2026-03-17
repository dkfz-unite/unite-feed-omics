using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Context;
using Unite.Indices.Entities.CnvProfiles;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class CnvProfileIndexingHandler: IndexingHandler<CnvProfileIndex, CnvProfileIndexingCache, CnvProfileIndexEntityBuilder, IndexingContext<CnvProfileIndex>>
{
    public CnvProfileIndexingHandler(TasksProcessingService taskProcessingService, 
        IDbContextFactory<DomainDbContext> dbContextFactory, 
        ILogger logger, 
        IIndexService<CnvProfileIndex> indexingService, 
        CnvProfileIndexEntityBuilder indexEntityBuilder) : base(taskProcessingService, dbContextFactory, logger, indexingService, indexEntityBuilder)
    {
    }

    protected override int BucketSize => 100;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.CNVProfile;
    protected override string IndexEntityKind => "CnvProfile";
}