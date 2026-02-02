using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

public class SampleSubmissionRepository(IMongoOptions options) : SubmissionRepository<SampleModel>(options, "meth");
