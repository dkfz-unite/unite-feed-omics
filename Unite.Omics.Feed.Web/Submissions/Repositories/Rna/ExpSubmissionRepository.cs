using Unite.Cache.Configuration.Options;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Rna;

public class ExpSubmissionRepository(IMongoOptions options) : SubmissionRepository(options)
{
    protected override string CollectionName => "rna_expressions";
}
