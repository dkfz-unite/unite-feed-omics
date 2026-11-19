using Unite.Cache.Configuration.Options;
using Unite.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sm;
using Unite.Omics.Feed.Web.Models.Dna;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class SmSubmissionRepository : SubmissionRepository<VariantAnalysisModel<VariantModel>>
{
    public SmSubmissionRepository(IMongoOptions options) : base(options, DataTypes.Omics.Dna.Sm)
    {
    }
}
