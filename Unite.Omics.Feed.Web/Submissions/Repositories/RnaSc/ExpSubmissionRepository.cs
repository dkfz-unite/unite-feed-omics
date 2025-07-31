using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

public class ExpSubmissionRepository : CacheRepository<AnalysisModel<EmptyModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "rnasc_expressions";

    public ExpSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
