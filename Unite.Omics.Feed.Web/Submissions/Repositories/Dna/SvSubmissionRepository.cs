using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sv;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class SvSubmissionRepository(IMongoOptions options) : SubmissionRepository(options)
{
    protected override string CollectionName => "dna_svs";
}

