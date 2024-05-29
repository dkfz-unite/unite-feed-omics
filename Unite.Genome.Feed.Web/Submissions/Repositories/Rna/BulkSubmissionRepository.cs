using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Rna;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Rna;

public class BulkSubmissionRepository : CacheRepository<SeqDataModel<BulkExpressionModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "rna_expressions";

    public BulkSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
