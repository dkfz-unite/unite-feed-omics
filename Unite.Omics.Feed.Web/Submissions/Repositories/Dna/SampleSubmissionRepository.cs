using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class SampleSubmissionRepository : CacheRepository<SampleModel>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "dna";

    public SampleSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
