using System.Diagnostics;
using Unite.Essentials.Extensions;
using Unite.Indices.Context;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public abstract class Indexer<TIndexEntity, TIndexingCache>: IIndexer<TIndexingCache>
    where TIndexEntity : class
    where TIndexingCache : IndexingCache
{
    private readonly IIndexService<TIndexEntity> _indexingService;
    private readonly IndexEntityBuilder<TIndexEntity, TIndexingCache> _indexEntityBuilder;
    private readonly ILogger _logger;

    protected Indexer(IIndexService<TIndexEntity> indexingService, 
        IndexEntityBuilder<TIndexEntity, TIndexingCache> indexEntityBuilder, 
        ILogger logger)
    {
        _indexingService = indexingService;
        _indexEntityBuilder = indexEntityBuilder;
        _logger = logger;
    }

    protected IIndexService<TIndexEntity> IndexingService => _indexingService;
    protected IndexEntityBuilder<TIndexEntity, TIndexingCache> IndexEntityBuilder => _indexEntityBuilder;
    protected ILogger Logger => _logger;
    
    public abstract string IndexEntityKind { get; }
    
    protected abstract string GetIndexEntityKey(TIndexEntity entity);

    public virtual async Task PrepareIndex()
    {
        await IndexingService.UpdateIndex();
    }

    public async Task BuildIndex(Unite.Data.Entities.Tasks.Task[] tasks, TIndexingCache indexingCache)
    {
        var stopwatch = new Stopwatch();
        
        var entitiesToDelete = new List<string>();
        var entitiesToCreate = new List<TIndexEntity>();

        tasks.ForEach(task =>
        {
            var id = int.Parse(task.Target);

            var indexEntities = IndexEntityBuilder.Create(id, indexingCache);

            if (indexEntities == null || indexEntities.Length == 0)
                entitiesToDelete.Add($"{id}");
            else
                entitiesToCreate.AddRange(indexEntities);

        });

        await DeleteIndexEntities(entitiesToDelete);
        await CreateIndexEntities(entitiesToCreate);
        
        stopwatch.Stop();
        Logger.LogInformation("Indexed {number} {entityKind} in {time}s", tasks.Length, IndexEntityKind, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));
    }

    protected virtual async Task DeleteIndexEntities(IList<string> entities)
    {
        if (entities.Any())
            await IndexingService.DeleteRange(entities);
    }

    protected virtual async Task CreateIndexEntities(List<TIndexEntity> entities)
    {
        if (entities.Any())
        {
            try
            {
                await IndexingService.AddRange(entities);
            }
            catch
            {
                foreach (var index in entities)
                {
                    try
                    {
                        await IndexingService.Add(index);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Failed to index {entityKind} {id}", IndexEntityKind, GetIndexEntityKey(index));
                    }
                }
            }
        }
    }
}