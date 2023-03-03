using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Transcriptomics;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Transcriptomics;

public class DefaultSubmissionRepository: CacheRepository<TranscriptomicsDataModel>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "transcriptomics";

    public DefaultSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
