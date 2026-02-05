using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers.RnaSc;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Models.Meth.Converters;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions.Repositories.Meth;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class MethLvlSubmissionHandler: SubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly LevelSubmissionRepository _submissionRepository;
    private readonly SampleIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly AnalysisModelConverter _converter = new();


    public MethLvlSubmissionHandler(
        HandlerPriority priority,
        AnalysisWriter dataWriter,
        SampleIndexingTaskService indexingTaskService,
        TasksProcessingService tasksProcessingService,
        LevelSubmissionRepository submissionRepository,
        ILogger<MethLvlSubmissionHandler> logger): base(priority)
    {
        _dataWriter = dataWriter;
        _indexingTaskService = indexingTaskService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;
        _submissionRepository = submissionRepository;
    }


    public override void Handle()
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

            _logger.LogInformation("Processed DNA Methylation data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionRepository.FindDocument(submissionId);
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _indexingTaskService.PopulateTasks(audit.Samples);
        _submissionRepository.Delete(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
