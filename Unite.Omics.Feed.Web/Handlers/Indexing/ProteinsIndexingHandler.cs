using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Indices.Context;
using Unite.Indices.Entities.Proteins;
using Unite.Omics.Indices.Services;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class ProteinsIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ProteinsIndexingCache _indexingCache;
    private readonly IIndexService<ProteinIndex> _proteinsIndexingService;
    private readonly IIndexService<ProteinExpressionIndex> _expressionsIndexingService;
    private readonly ILogger _logger;


    public ProteinsIndexingHandler(
        TasksProcessingService taskProcessingService,
        ProteinsIndexingCache indexingCache,
        IIndexService<ProteinIndex> proteinsIndexingService,
        IIndexService<ProteinExpressionIndex> expressionsIndexingService,
        ILogger<GenesIndexingHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _indexingCache = indexingCache;
        _proteinsIndexingService = proteinsIndexingService;
        _expressionsIndexingService = expressionsIndexingService;
        _logger = logger;
    }


    public async Task Prepare()
    {
        await _proteinsIndexingService.CreateIndex();
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
        
        await _taskProcessingService.Process(IndexingTaskType.Protein, bucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            _indexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var proteinIndicesToDelete = new List<string>();
            var proteinIndicesToCreate = new List<ProteinIndex>();
            var proteinIndexCreator = new ProteinIndexCreator(_indexingCache);

            var expressionIndicesToCreate = new List<ProteinExpressionIndex>();
            var expressionIndexCreator = new ProteinExpressionIndexCreator(_indexingCache);

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                var proteinIndex = proteinIndexCreator.CreateIndex(id);
                var expressionIndices = expressionIndexCreator.CreateIndices(id);

                if (proteinIndex != null)
                    proteinIndicesToCreate.Add(proteinIndex);
                else
                    proteinIndicesToDelete.Add(task.Target);
                
                if (expressionIndices != null)
                    expressionIndicesToCreate.AddRange(expressionIndices);

            });

            if (proteinIndicesToDelete.Any())
            {
                await _proteinsIndexingService.DeleteRange(proteinIndicesToDelete);
                await _expressionsIndexingService.DeleteWhereEquals(index => index.Protein.Id, proteinIndicesToDelete.Select(id => int.Parse(id)).ToArray());
            }

            if (proteinIndicesToCreate.Any())
                await _proteinsIndexingService.AddRange(proteinIndicesToCreate);

            if (expressionIndicesToCreate.Any())
                await _expressionsIndexingService.AddRange(expressionIndicesToCreate);

            _indexingCache.Clear();

            stopwatch.Stop();

            _logger.LogInformation("Indexed {number} proteins in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
