using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Dna.Sm;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Dna;

public class SmSubmissionRepository : CacheRepository<AnalysisModel<VariantModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "dna_sms";

    public SmSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
