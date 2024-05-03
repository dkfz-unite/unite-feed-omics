using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Variants.CNV;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Variants;

public class CnvSubmissionRepository : CacheRepository<SequencingDataModel<VariantModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "variants_cnv";

    public CnvSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
