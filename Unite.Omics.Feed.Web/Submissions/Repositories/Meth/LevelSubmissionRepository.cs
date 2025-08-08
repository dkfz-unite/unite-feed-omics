using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

public class LevelSubmissionRepository : CacheRepository<AnalysisModel<EmptyModel>>
{ 
    public override string DatabaseName => "submissions";
    public override string CollectionName => "meth_lvl";

    public LevelSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
