using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sm;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class SmSubmissionRepository(IMongoOptions options) : SubmissionRepository<AnalysisModel<VariantModel>>(options, "dna_sms");
