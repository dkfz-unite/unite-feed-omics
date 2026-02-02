using Unite.Cache.Configuration.Options;

namespace Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

public class CnvProfileSubmissionRepository(IMongoOptions options) : SubmissionRepository(options, "dna_cnv_profiles");