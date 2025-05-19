using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Rna;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Rna;

public class ExpSubmissionRepository : CacheRepository<AnalysisModel<ExpressionModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "rna_expressions";

    public ExpSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
