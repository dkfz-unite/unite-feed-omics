using Unite.Cache.Configuration.Options;
using Unite.Cache.Repositories;

namespace Unite.Omics.Feed.Web.Submissions;

public abstract class SubmissionRepository<TModel>(IMongoOptions options, string collectionName) : CacheRepository<TModel>(options) 
    where TModel : class
{
    public override string DatabaseName => "submissions";
    public override string CollectionName => collectionName;
}