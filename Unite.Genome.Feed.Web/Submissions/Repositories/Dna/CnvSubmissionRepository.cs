using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Dna.Cnv;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Dna;

public class CnvSubmissionRepository : CacheRepository<SeqDataModel<VariantModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "dna_cnvs";

    public CnvSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
