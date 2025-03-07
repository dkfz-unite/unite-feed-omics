using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Dna.Meth;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Dna;

public class MethSubmissionRepository : CacheRepository<AnalysisModel<ExpressionModel>>
{ 
    public override string DatabaseName => "submissions";
    public override string CollectionName => "dna_meth";

    public MethSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
