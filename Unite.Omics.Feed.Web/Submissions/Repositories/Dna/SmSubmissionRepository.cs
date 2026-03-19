using Unite.Cache.Configuration.Options;
using Unite.Data.Constants;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Dna.Sm;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class SmSubmissionRepository : SubmissionRepository<AnalysisModel<VariantModel>>
{
    public SmSubmissionRepository(IMongoOptions options) : base(options, DataTypes.Omics.Dna.Sm)
    {
    }
}
