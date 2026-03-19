using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers;
using Unite.Omics.Feed.Web.Models.Base.Converters;
using Unite.Omics.Feed.Web.Services.Indexing;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public abstract class SampleSubmissionHandler: SubmissionHandler
{
    protected readonly SampleWriter _dataWriter;
    protected readonly TasksProcessingService _taskProcessingService;
    protected readonly SampleIndexingTaskService _indexingTaskService;
    protected readonly ILogger _logger;

    protected readonly SampleModelConverter _converter = new();

    protected abstract SubmissionTaskType TaskType { get; }


    public SampleSubmissionHandler(
        HandlerPriority priority,
        SampleWriter dataWriter,
        TasksProcessingService taskProcessingService,
        SampleIndexingTaskService indexingTaskService,
        ILogger logger): base(priority)
    {
        _dataWriter = dataWriter;
        _taskProcessingService = taskProcessingService;
        _indexingTaskService = indexingTaskService;
        _logger = logger;
    }


    public override Task Handle()
    {
        return Task.Run(ProcessSubmissionTasks);
    }


    protected virtual void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(TaskType, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed {type} sample data submission in {time}s", GetTypeName(TaskType), Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    protected virtual void ProcessSubmission(string submissionId)
    {
        var submittedData = FindSubmission(submissionId);
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _indexingTaskService.PopulateTasks(audit.Samples);
        DeleteSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }

    protected abstract Models.Base.SampleModel FindSubmission(string submissionId);

    protected abstract void DeleteSubmission(string submissionId);


    private static string GetTypeName(SubmissionTaskType taskType)
    {
        return taskType switch
        {
            SubmissionTaskType.DNA => "DNA",
            SubmissionTaskType.METH => "Methylation",
            SubmissionTaskType.RNA => "RNA",
            SubmissionTaskType.RNASC => "scRNA",
            _ => throw new NotSupportedException($"Unsupported task type: {taskType}"),
        };
    }
}
