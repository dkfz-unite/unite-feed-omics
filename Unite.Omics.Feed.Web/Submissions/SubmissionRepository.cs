using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;

namespace Unite.Omics.Feed.Web.Submissions;

public abstract class SubmissionRepository(IMongoOptions options) : CacheRepositoryGenericPerMethod(options)
{
    protected override string DatabaseName => "submissions";
}