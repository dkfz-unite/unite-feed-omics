using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Transcriptomics;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Transcriptomics;

public class BulkSubmissionRepository : CacheRepository<SequencingDataModel<BulkExpressionModel>>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "gene_expressions_bulk";

    public BulkSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
