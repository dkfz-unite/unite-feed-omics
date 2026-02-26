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
    IIndexService<CnvIndex> indexingService,
    VariantIndexingCache<Variant, VariantEntry> indexingCache,
    IIndexCreator<CnvIndex> indexCreator,
    ILogger<CnvsIndexingHandler> logger)
    : IndexingHandler<CnvIndex>(taskProcessingService, indexingService, indexingCache, indexCreator, logger)
{
    protected override async Task ProcessIndexingTasks()
    {
        if (TaskProcessingService.HasTasks(WorkerType.Submission) || TaskProcessingService.HasTasks(WorkerType.Annotation))
            return;
                
        var stopwatch = new Stopwatch();

        await TaskProcessingService.Process(IndexingTaskType.CNV, options.CnvBucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            IndexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<CnvIndex>();

            tasks.ForEach(task =>
            {
                var id = int.Parse(task.Target);

                var index = IndexCreator.Create(id);

                if (index == null)
                    indicesToDelete.Add($"{id}");
                else
                    indicesToCreate.Add(index);
            });

            if (indicesToDelete.Any())
                await IndexingService.DeleteRange(indicesToDelete);
            
            if (indicesToCreate.Any())
                await IndexingService.AddRange(indicesToCreate);

            IndexingCache.Clear();

            stopwatch.Stop();

            Logger.LogInformation("Indexed {number} CNVs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
