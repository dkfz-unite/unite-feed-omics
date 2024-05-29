using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Rna;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Rna;

public class CellSubmissionRepository : CacheRepository<SeqDataModel<CellExpressionModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "rnasc_expressions";

    public CellSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
