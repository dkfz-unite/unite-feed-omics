using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Dna.Ssm;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Dna;

public class SsmSubmissionRepository : CacheRepository<AnalysisModel<VariantModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "dna_ssms";

    public SsmSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
