using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Dna.Sv;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Dna;

public class SvSubmissionRepository : CacheRepository<AnalysisModel<VariantModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "dna_svs";

    public SvSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
