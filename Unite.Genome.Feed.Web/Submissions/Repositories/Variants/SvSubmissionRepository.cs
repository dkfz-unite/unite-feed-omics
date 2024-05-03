using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Variants.SV;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Variants;

public class SvSubmissionRepository : CacheRepository<SequencingDataModel<VariantModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "variants_sv";

    public SvSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
