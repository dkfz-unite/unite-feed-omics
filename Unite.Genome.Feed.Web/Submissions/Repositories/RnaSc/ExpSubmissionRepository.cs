using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.RnaSc;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.RnaSc;

public class ExpSubmissionRepository : CacheRepository<AnalysisModel<ExpressionModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "rnasc_expressions";

    public ExpSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
