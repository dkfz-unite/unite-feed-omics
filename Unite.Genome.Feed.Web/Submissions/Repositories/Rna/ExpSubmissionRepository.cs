using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Rna;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Rna;

public class ExpSubmissionRepository : CacheRepository<AnalysisModel<ExpressionModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "rna_expressions";

    public ExpSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
