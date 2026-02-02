using Unite.Cache.Configuration.Options;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

public class SampleSubmissionRepository(IMongoOptions options) : SubmissionRepository(options, "meth");
