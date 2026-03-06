using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

public class ExpSubmissionRepository : SubmissionRepository<AnalysisModel<EmptyModel>>
{
    public ExpSubmissionRepository(IMongoOptions options) : base(options, "rnasc_expressions")
    {
    }
}