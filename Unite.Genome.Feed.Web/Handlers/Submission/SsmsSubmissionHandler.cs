using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Data.Writers.Variants;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class SsmsSubmissionHandler
{
    private readonly VariantsDataWriter _dataWriter;
    private readonly SsmAnnotationTaskService _annotationTaskService;
    private readonly SsmIndexingTaskService _indexingTaskService;
    private readonly VariantsSubmissionService _submissionService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Variants.SSM.Converters.SequencingDataModelConverter _converter;


    public SsmsSubmissionHandler(
        VariantsDataWriter dataWriter,
        SsmAnnotationTaskService annotationTaskService,
        SsmIndexingTaskService indexingTaskService,
        VariantsSubmissionService submissionService,
        TasksProcessingService tasksProcessingService,
        ILogger<SsmsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _indexingTaskService = indexingTaskService;
        _submissionService = submissionService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;

        _converter = new Models.Variants.SSM.Converters.SequencingDataModelConverter();
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }


    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.SSM, 1, (tasks) =>
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
        var submittedSequencingData = _submissionService.FindSsmSubmission(submissionId);
        var convertedSequencingData = _converter.Convert(submittedSequencingData);

        _dataWriter.SaveData(convertedSequencingData, out var audit);
        _annotationTaskService.PopulateTasks(audit.Ssms);
        _indexingTaskService.PopulateTasks(audit.SsmsEntries.Except(audit.Ssms));
        _submissionService.DeleteSsmSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
