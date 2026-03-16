using Unite.Cache.Configuration.Options;
using Unite.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sv;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class SvSubmissionRepository : SubmissionRepository<AnalysisModel<VariantModel>>
{
    public SvSubmissionRepository(IMongoOptions options) : base(options, DataTypes.Omics.Dna.Sv)
    {
    }
}

