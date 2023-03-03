using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Variants;
using Unite.Genome.Feed.Web.Models.Variants.CNV;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Variants.CNV;

public class DefaultSubmissionRepository : CacheRepository<SequencingDataModel<VariantModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "variants_cnv";

    public DefaultSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
