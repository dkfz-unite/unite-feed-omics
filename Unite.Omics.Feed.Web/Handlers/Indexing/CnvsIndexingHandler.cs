using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class CnvsIndexingHandler(
    VariantsIndexingOptions options,
    TasksProcessingService taskProcessingService,
    VariantIndexingCache<Variant, VariantEntry> indexingCache,
    IIndexService<CnvIndex> indexingService,
    ILogger<CnvsIndexingHandler> logger)
    : IndexingHandler
{
    public override async Task Prepare()
    {
        await indexingService.UpdateIndex();
    }

    public override async Task Handle()
    {
        await ProcessIndexingTasks(options.CnvBucketSize);
    }

    private async Task ProcessIndexingTasks(int bucketSize)
    {
        if (taskProcessingService.HasTasks(WorkerType.Submission) || taskProcessingService.HasTasks(WorkerType.Annotation))
            return;
                
        var stopwatch = new Stopwatch();

        await taskProcessingService.Process(IndexingTaskType.CNV, bucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            indexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<CnvIndex>();
            var indexCreator = new CnvIndexCreator(indexingCache);

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                var index = indexCreator.CreateIndex(id);

                if (index == null)
                    indicesToDelete.Add($"{id}");
                else
                    indicesToCreate.Add(index);
            });

            if (indicesToDelete.Any())
                await indexingService.DeleteRange(indicesToDelete);
            
            if (indicesToCreate.Any())
                await indexingService.AddRange(indicesToCreate);

            indexingCache.Clear();

            stopwatch.Stop();

            logger.LogInformation("Indexed {number} CNVs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
