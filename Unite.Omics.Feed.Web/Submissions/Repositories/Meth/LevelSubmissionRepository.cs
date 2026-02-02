using Unite.Cache.Configuration.Options;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

public class LevelSubmissionRepository(IMongoOptions options) : SubmissionRepository(options, "meth_lvl");
