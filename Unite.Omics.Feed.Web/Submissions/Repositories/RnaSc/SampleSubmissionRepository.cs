using Unite.Cache.Configuration.Options;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

public class SampleSubmissionRepository(IMongoOptions options) : SubmissionRepository(options, "rnasc");
