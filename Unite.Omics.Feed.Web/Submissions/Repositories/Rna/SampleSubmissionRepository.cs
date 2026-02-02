using Unite.Cache.Configuration.Options;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Rna;

public class SampleSubmissionRepository(IMongoOptions options) : SubmissionRepository(options, "rna");
