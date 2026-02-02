using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Rna;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Rna;

public class ExpSubmissionRepository(IMongoOptions options) : SubmissionRepository<AnalysisModel<ExpressionModel>>(options, "rna_expressions");
