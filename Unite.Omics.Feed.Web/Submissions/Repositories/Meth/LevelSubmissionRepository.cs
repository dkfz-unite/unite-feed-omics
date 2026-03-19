using Unite.Cache.Configuration.Options;
using Unite.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

public class LevelSubmissionRepository : SubmissionRepository<AnalysisModel<EmptyModel>>
{
    public LevelSubmissionRepository(IMongoOptions options) : base(options, DataTypes.Omics.Methylation.Level)
    {
    }
}
