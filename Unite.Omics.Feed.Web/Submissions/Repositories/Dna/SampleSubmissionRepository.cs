using Unite.Cache.Configuration.Options;
using Unite.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class SampleSubmissionRepository : SubmissionRepository<SampleModel>
{
    public SampleSubmissionRepository(IMongoOptions options) : base(options, DataTypes.Omics.Dna.Sample)
    {
    }
}
