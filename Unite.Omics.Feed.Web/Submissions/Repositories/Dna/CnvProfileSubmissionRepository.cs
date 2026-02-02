using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.CnvProfile;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class CnvProfileSubmissionRepository(IMongoOptions options) : SubmissionRepository<AnalysisModel<CnvProfileModel>>(options, "dna_cnv_profiles");