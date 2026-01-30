using Unite.Cache.Configuration.Options;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

public class ExpSubmissionRepository(IMongoOptions options) : SubmissionRepository(options)
{
    protected override string CollectionName => "rnasc_expressions";
}
