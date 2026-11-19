using Unite.Cache.Configuration.Options;
using Unite.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sv;
using Unite.Omics.Feed.Web.Models.Dna;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class SvSubmissionRepository : SubmissionRepository<VariantAnalysisModel<VariantModel>>
{
    public SvSubmissionRepository(IMongoOptions options) : base(options, DataTypes.Omics.Dna.Sv)
    {
    }
}

