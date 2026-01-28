using Unite.Cache.Configuration.Options;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class CnvSubmissionRepository(IMongoOptions options) : SubmissionRepository(options)
{
    protected override string CollectionName => "dna_cnvs";
}
