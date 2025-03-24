using System;
using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;
using Unite.Genome.Feed.Web.Models.Base;

namespace Unite.Genome.Feed.Web.Submissions.Repositories.Rna;

public class SampleSubmissionRepository : CacheRepository<SampleModel>
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => "rna";

    public SampleSubmissionRepository(IMongoOptions options) : base(options)
    {
    }
}
