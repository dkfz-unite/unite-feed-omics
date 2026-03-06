using Unite.Cache.Configuration.Options;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.CnvProfile;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class CnvProfileSubmissionRepository : SubmissionRepository<AnalysisModel<CnvProfileModel>>
{
    public CnvProfileSubmissionRepository(IMongoOptions options) : base(options, "dna_cnv_profiles")
    {
    }
}