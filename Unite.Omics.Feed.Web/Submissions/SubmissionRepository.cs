using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;

namespace Unite.Omics.Feed.Web.Submissions;

public abstract class SubmissionRepository<TModel> : CacheRepository<TModel> 
    where TModel : class
{
    private readonly string _collectionName;

    protected SubmissionRepository(IMongoOptions options, string collectionName) : base(options)
    {
        _collectionName = collectionName;
    }

    public override string DatabaseName => "submissions";
    public override string CollectionName => _collectionName;
}