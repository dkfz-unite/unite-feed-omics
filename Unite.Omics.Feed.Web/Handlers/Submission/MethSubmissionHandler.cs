using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers.RnaSc;
using Unite.Omics.Feed.Web.Models.Meth.Converters;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class MethSubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly MethSubmissionService _submissionService;
    private readonly SampleIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly AnalysisModelConverter _converter = new();


    public MethSubmissionHandler(
        AnalysisWriter dataWriter,
        MethSubmissionService submissionService,
        SampleIndexingTaskService indexingTaskService,
        TasksProcessingService tasksProcessingService,
        ILogger<MethSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _submissionService = submissionService;
        _indexingTaskService = indexingTaskService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }


    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.METH_LVL, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed DNA Methylation data submission in  submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionService.FindLevelSubmission(submissionId);
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _indexingTaskService.PopulateTasks(audit.Samples);
        _submissionService.DeleteLevelSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
