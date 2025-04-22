using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Data.Writers.Dna;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class SmsSubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly SmAnnotationTaskService _annotationTaskService;
    private readonly SmIndexingTaskService _indexingTaskService;
    private readonly DnaSubmissionService _submissionService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Dna.Sm.Converters.AnalysisModelConverter _converter;


    public SmsSubmissionHandler(
        AnalysisWriter dataWriter,
        SmAnnotationTaskService annotationTaskService,
        SmIndexingTaskService indexingTaskService,
        DnaSubmissionService submissionService,
        TasksProcessingService tasksProcessingService,
        ILogger<SmsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _indexingTaskService = indexingTaskService;
        _submissionService = submissionService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;

        _converter = new Models.Dna.Sm.Converters.AnalysisModelConverter();
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }


    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.DNA_SM, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed SMs data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionService.FindSmSubmission(submissionId);
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _annotationTaskService.PopulateTasks(audit.Sms);
        _indexingTaskService.PopulateTasks(audit.SmsEntries.Except(audit.Sms));
        _submissionService.DeleteSmSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
