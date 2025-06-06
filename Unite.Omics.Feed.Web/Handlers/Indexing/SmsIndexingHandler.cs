﻿using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services;
using Unite.Indices.Context;
using Unite.Indices.Entities.Variants;

namespace Unite.Omics.Feed.Web.Handlers.Indexing;

public class SmsIndexingHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly VariantIndexingCache<Variant, VariantEntry> _indexingCache;
    private readonly IIndexService<SmIndex> _indexingService;
    private readonly ILogger _logger;


    public SmsIndexingHandler(
        TasksProcessingService taskProcessingService,
        VariantIndexingCache<Variant, VariantEntry> indexingCache,
        IIndexService<SmIndex> indexingService,
        ILogger<SmsIndexingHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _indexingCache = indexingCache;
        _indexingService = indexingService;
        _logger = logger;
    }


    public async Task Prepare()
    {
        await _indexingService.UpdateIndex();
    }

    public async Task Handle(int bucketSize)
    {
        await ProcessIndexingTasks(bucketSize);
    }


    private async Task ProcessIndexingTasks(int bucketSize)
    {
        var stopwatch = new Stopwatch();

        await _taskProcessingService.Process(IndexingTaskType.SM, bucketSize, async (tasks) =>
        {
            if (_taskProcessingService.HasTasks(WorkerType.Submission) || _taskProcessingService.HasTasks(WorkerType.Annotation))
                return false;

            stopwatch.Restart();

            _indexingCache.Load(tasks.Select(task => int.Parse(task.Target)).ToArray());

            var indexCreator = new SmIndexCreator(_indexingCache);
            var indicesToDelete = new List<string>();
            var indicesToCreate = new List<SmIndex>();

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
                await _indexingService.DeleteRange(indicesToDelete);

            if (indicesToCreate.Any())
                await _indexingService.AddRange(indicesToCreate);

            _indexingCache.Clear();

            stopwatch.Stop();

            _logger.LogInformation("Indexed {number} SMs in {time}s", tasks.Length, Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
}
