using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Meth;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Meth;

public class LevelSubmissionRepository : CacheRepository<AnalysisModel<LevelModel>>
{ 
    public override string DatabaseName => "submissions";
    public override string CollectionName => "meth_lvl";

    public LevelSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
