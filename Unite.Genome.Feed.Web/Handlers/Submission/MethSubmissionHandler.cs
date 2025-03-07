using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Data.Writers.RnaSc;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class MethSubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly DnaSubmissionService _submissionService;
    private readonly SampleIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Dna.Meth.Converters.AnalysisModelConverter _converter = new();


    public MethSubmissionHandler(
        AnalysisWriter dataWriter,
        DnaSubmissionService submissionService,
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

        _taskProcessingService.Process(SubmissionTaskType.DNA_METH, TaskStatusType.Prepared, 1, (tasks) =>
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
        var submittedData = _submissionService.FindMethSubmission(submissionId);
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _indexingTaskService.PopulateTasks(audit.Samples);
        _submissionService.DeleteMethSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
