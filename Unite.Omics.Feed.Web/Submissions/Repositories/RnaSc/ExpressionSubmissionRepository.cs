using Unite.Cache.Configuration.Options;
using Unite.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.RnaSc;

public class ExpressionSubmissionRepository(IMongoOptions options) : SubmissionRepository<AnalysisModel<EmptyModel>>(options, DataTypes.Omics.Rnasc.Expression);