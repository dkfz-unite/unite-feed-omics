using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;
using Unite.Omics.Feed.Web.Configuration.Options;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SvsIndexingHandler(
    VariantsIndexingOptions options,
    TasksProcessingService taskProcessingService,
    VariantIndexingCache<Variant, VariantEntry> indexingCache,
    IIndexService<SvIndex> indexingService,
    IIndexCreator<SvIndex> indexCreator,
    ILogger<SvsIndexingHandler> logger)
    : IndexingHandler<SvIndex>(taskProcessingService, indexingService, indexingCache, indexCreator, logger)
{
    public override async Task Handle()
    {
        await ProcessIndexingTasks(options.SvBucketSize);
    }

    private async Task ProcessIndexingTasks(int bucketSize)
    {
        if (TaskProcessingService.HasTasks(WorkerType.Submission) || TaskProcessingService.HasTasks(WorkerType.Annotation))
            return;

        var stopwatch = new Stopwatch();

        await TaskProcessingService.Process(IndexingTaskType.SV, bucketSize, async (tasks) =>
        {
            stopwatch.Restart();

            IndexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<SvIndex>();

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

            Logger.LogInformation("Indexed {number} SVs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
