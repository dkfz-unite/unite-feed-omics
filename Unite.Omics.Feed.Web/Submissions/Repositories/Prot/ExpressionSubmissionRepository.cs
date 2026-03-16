using Unite.Cache.Configuration.Options;
using Unite.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Prot;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Prot;

public class ExpressionSubmissionRepository(IMongoOptions options) : SubmissionRepository<AnalysisModel<ExpressionModel>>(options, DataTypes.Omics.Proteomics.Expression);
