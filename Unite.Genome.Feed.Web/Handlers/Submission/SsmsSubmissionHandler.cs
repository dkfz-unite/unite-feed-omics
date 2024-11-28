using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Data.Writers.Dna;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class SsmsSubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly SsmAnnotationTaskService _annotationTaskService;
    private readonly SsmIndexingTaskService _indexingTaskService;
    private readonly DnaSubmissionService _submissionService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Dna.Ssm.Converters.AnalysisModelConverter _converter;


    public SsmsSubmissionHandler(
        AnalysisWriter dataWriter,
        SsmAnnotationTaskService annotationTaskService,
        SsmIndexingTaskService indexingTaskService,
        DnaSubmissionService submissionService,
        TasksProcessingService tasksProcessingService,
        ILogger<SsmsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _indexingTaskService = indexingTaskService;
        _submissionService = submissionService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;

        _converter = new Models.Dna.Ssm.Converters.AnalysisModelConverter();
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }


    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.DNA_SSM, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed SSMs data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionService.FindSsmSubmission(submissionId);
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _annotationTaskService.PopulateTasks(audit.Ssms);
        _indexingTaskService.PopulateTasks(audit.SsmsEntries.Except(audit.Ssms));
        _submissionService.DeleteSsmSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
