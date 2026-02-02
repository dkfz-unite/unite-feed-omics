using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sv;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class SvSubmissionRepository(IMongoOptions options) : SubmissionRepository<AnalysisModel<VariantModel>>(options, "dna_svs");

