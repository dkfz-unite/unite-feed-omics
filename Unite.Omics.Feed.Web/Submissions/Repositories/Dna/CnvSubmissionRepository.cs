using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Cnv;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class CnvSubmissionRepository : CacheRepository<AnalysisModel<VariantModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "dna_cnvs";

    public CnvSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
