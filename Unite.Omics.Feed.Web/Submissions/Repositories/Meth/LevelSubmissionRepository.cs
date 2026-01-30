using Unite.Cache.Configuration.Options;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

public class LevelSubmissionRepository(IMongoOptions options) : SubmissionRepository(options)
{
    protected override string CollectionName => "meth_lvl";
}
