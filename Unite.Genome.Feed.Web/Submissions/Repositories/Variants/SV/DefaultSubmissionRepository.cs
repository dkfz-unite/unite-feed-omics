using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Variants;
using Unite.Genome.Feed.Web.Models.Variants.SV;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Variants.SV;

public class DefaultSubmissionRepository : CacheRepository<SequencingDataModel<VariantModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "variants_sv";

    public DefaultSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
