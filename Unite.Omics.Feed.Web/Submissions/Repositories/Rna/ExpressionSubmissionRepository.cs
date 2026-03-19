using Unite.Cache.Configuration.Options;
using Unite.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Rna;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Rna;

public class ExpressionSubmissionRepository(IMongoOptions options) : SubmissionRepository<AnalysisModel<ExpressionModel>>(options, DataTypes.Omics.Rna.Expression);
