using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers.RnaSc;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class CellGeneExpSubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly RnaScSubmissionService _submissionService;
    private readonly SampleIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.RnaSc.Converters.AnalysisModelConverter _converter = new();


    public CellGeneExpSubmissionHandler(
        AnalysisWriter dataWriter,
        RnaScSubmissionService submissionService,
        SampleIndexingTaskService indexingTaskService,
        TasksProcessingService tasksProcessingService,
        ILogger<CellGeneExpSubmissionHandler> logger)
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

        _taskProcessingService.Process(SubmissionTaskType.RNASC_EXP, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed single-cell transcriptomics data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionService.FindExpSubmission(submissionId);
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _indexingTaskService.PopulateTasks(audit.Samples);
        _submissionService.DeleteExpSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
