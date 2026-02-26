using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Indices.Context;
using Unite.Indices.Entities.CnvProfiles;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class CnvProfilesIndexingHandler(
    TasksProcessingService taskProcessingService,
    IIndexService<CnvProfileIndex> indexingService,
    CnvProfileIndexingCache indexingCache,
    IIndexCreator<CnvProfileIndex> indexCreator,
    ILogger<GenesIndexingHandler> logger)
    : IndexingHandler<CnvProfileIndex>(taskProcessingService, indexingService, indexingCache, indexCreator, logger)
{
    protected override int BucketSize => 100;
    protected override IndexingTaskType IndexingTaskType => IndexingTaskType.CNVProfile;
    protected override string IndexEntityKind => "CnvProfile";
    protected override string GetIndexEntityKey(CnvProfileIndex entity)
    {
        return entity.Id.ToString();
    }
}