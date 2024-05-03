using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Variants.SSM;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Variants;

public class SsmSubmissionRepository : CacheRepository<SequencingDataModel<VariantModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "variants_ssm";

    public SsmSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
