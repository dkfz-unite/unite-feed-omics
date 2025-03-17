using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.RnaSc;

public class SampleSubmissionRepository : CacheRepository<SampleModel>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "rnasc";

    public SampleSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
