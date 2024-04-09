using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Data.Writers.Variants;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class SsmsSubmissionHandler
{
    private readonly VariantsDataWriter _dataWriter;
    private readonly SsmAnnotationTaskService _annotationTaskService;
    private readonly VariantsSubmissionService _submissionService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Variants.SSM.Converters.SequencingDataModelConverter _converter;


    public SsmsSubmissionHandler(
        VariantsDataWriter dataWriter,
        SsmAnnotationTaskService annotationTaskService,
        VariantsSubmissionService submissionService,
        TasksProcessingService tasksProcessingService,
        ILogger<SsmsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
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
            _logger.LogInformation($"Processing SSM data submission");

            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processing of SSM data submission completed in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedSequencingData = _submissionService.FindSsmSubmission(submissionId);
        var convertedSequencingData = _converter.Convert(submittedSequencingData);

        _dataWriter.SaveData(convertedSequencingData, out var audit);
        _annotationTaskService.PopulateTasks(audit.Ssms);
        _submissionService.DeleteSsmSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
