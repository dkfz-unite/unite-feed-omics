using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Genes;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class GenesIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly GenesIndexingCache _indexingCache;
    private readonly IIndexService<GeneIndex> _genesIndexingService;
    private readonly IIndexService<GeneExpressionIndex> _expressionsIndexingService;
    private readonly ILogger _logger;


    public GenesIndexingHandler(
        TasksProcessingService taskProcessingService,
        GenesIndexingCache indexingCache,
        IIndexService<GeneIndex> genesIndexingService,
        IIndexService<GeneExpressionIndex> expressionsIndexingService,
        ILogger<GenesIndexingHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _indexingCache = indexingCache;
        _genesIndexingService = genesIndexingService;
        _expressionsIndexingService = expressionsIndexingService;
        _logger = logger;
    }


    public async Task Prepare()
    {
        await _genesIndexingService.CreateIndex();
        await _expressionsIndexingService.CreateIndex();
        // await _indexingService.UpdateIndex();
    }

    public async Task Handle(int bucketSize)
    {
        await ProcessIndexingTasks(bucketSize);
    }


    private async Task ProcessIndexingTasks(int bucketSize)
    {
        if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
            return;

        var stopwatch = new Stopwatch();
        
        await _taskProcessingService.Process(IndexingTaskType.Gene, bucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            _indexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var geneIndicesToDelete = new List<string>();
            var geneIndicesToCreate = new List<GeneIndex>();
            var geneIndexCreator = new GeneIndexCreator(_indexingCache);

            var expressionIndicesToCreate = new List<GeneExpressionIndex>();
            var expressionIndexCreator = new GeneExpressionIndexCreator(_indexingCache);

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                var geneIndex = geneIndexCreator.CreateIndex(id);
                var expressionIndices = expressionIndexCreator.CreateIndices(id);


                if (geneIndex != null)
                    geneIndicesToCreate.Add(geneIndex);
                else
                    geneIndicesToDelete.Add($"{id}");

                if (expressionIndices != null)
                    expressionIndicesToCreate.AddRange(expressionIndices);

            });

            if (geneIndicesToDelete.Any())
            {
                await _genesIndexingService.DeleteRange(geneIndicesToDelete);
                await _expressionsIndexingService.DeleteWhereEquals(index => index.Gene.Id, geneIndicesToDelete.Select(id => int.Parse(id)).ToArray());
            }

            if (geneIndicesToCreate.Any())
                await _genesIndexingService.AddRange(geneIndicesToCreate);

            if (expressionIndicesToCreate.Any())
                await _expressionsIndexingService.AddRange(expressionIndicesToCreate);

            _indexingCache.Clear();

            stopwatch.Stop();

            _logger.LogInformation("Indexed {number} genes and in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
