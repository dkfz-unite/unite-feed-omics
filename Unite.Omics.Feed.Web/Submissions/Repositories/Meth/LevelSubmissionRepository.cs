using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

public class LevelSubmissionRepository : SubmissionRepository<AnalysisModel<EmptyModel>>
{
    public LevelSubmissionRepository(IMongoOptions options) : base(options, "meth_lvl")
    {
    }
}
