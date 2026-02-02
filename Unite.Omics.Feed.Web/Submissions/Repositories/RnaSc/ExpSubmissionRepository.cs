using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

public class ExpSubmissionRepository(IMongoOptions options) : SubmissionRepository<AnalysisModel<EmptyModel>>(options, "rnasc_expressions");