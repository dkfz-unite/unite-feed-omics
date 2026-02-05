using Unite.Cache.Configuration.Options;
using Unite.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Cnv;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class CnvSubmissionRepository(IMongoOptions options) : SubmissionRepository<AnalysisModel<VariantModel>>(options, DataTypes.Omics.Dna.Cnv);
