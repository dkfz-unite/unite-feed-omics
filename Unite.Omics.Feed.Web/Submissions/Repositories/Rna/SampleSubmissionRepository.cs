using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Rna;

public class SampleSubmissionRepository : SubmissionRepository<SampleModel>
{
    public SampleSubmissionRepository(IMongoOptions options) : base(options, "rna")
    {
    }
}
