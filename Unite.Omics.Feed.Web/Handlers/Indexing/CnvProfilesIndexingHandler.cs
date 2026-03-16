using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Entities.CnvProfiles;
using Unite.Omics.Feed.Web.Handlers.Indexing.Indexers;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class CnvProfilesIndexingHandler : IndexingHandler<CnvProfileIndexingCache>
{
    private readonly CnvProfileIndexer _cnvProfileIndexer;
    
    public CnvProfilesIndexingHandler(TasksProcessingService taskProcessingService,
        IDbContextFactory<DomainDbContext> dbContextFactory,
        CnvProfileIndexer cnvProfileIndexer,
        ILogger<GenesIndexingHandler> logger) : base(taskProcessingService, dbContextFactory, logger)
    {
        _cnvProfileIndexer = cnvProfileIndexer;
    }

    protected override IIndexer<CnvProfileIndexingCache>[] Indexers =>
    [
        _cnvProfileIndexer
    ];
    
    protected override int BucketSize => 100;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.CNVProfile;
}