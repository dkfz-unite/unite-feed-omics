using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;

namespace Unite.Omics.Indices.Services;

public abstract class IndexingCache(IDbContextFactory<DomainDbContext> dbContextFactory) : IDisposable
{
    public IDbContextFactory<DomainDbContext> DbContextFactory { get; } = dbContextFactory;

    public static T Create<T>(
        IDbContextFactory<DomainDbContext> dbContextFactory,
        int[] ids
    ) where T : IndexingCache
    {
        var instance = (T)Activator.CreateInstance(typeof(T), dbContextFactory)!;
        instance.Initialize(ids);
        return instance;
    }
    
    private void Initialize(int[] ids) => Load(ids);

    protected abstract void Load(int[] ids);
    
    public virtual void Dispose()
    { }
}