using System.Diagnostics;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Genome.Feed.Data;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class MutationsSubmissionHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly VariantsSubmissionService _submissionService;
    private readonly SequencingDataWriter _dataWriter;
    private readonly MutationAnnotationTaskService _annotationTaskService;
    private readonly ILogger _logger;

    private readonly Models.Variants.SSM.Converters.SequencingDataModelConverter _dataConverter;


    public MutationsSubmissionHandler(
        TasksProcessingService tasksProcessingService,
        VariantsSubmissionService submissionService,
        SequencingDataWriter dataWriter,
        MutationAnnotationTaskService annotationTaskService,
        ILogger<MutationsSubmissionHandler> logger)
    {
        _taskProcessingService = tasksProcessingService;
        _submissionService = submissionService;
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _logger = logger;

        _dataConverter = new Models.Variants.SSM.Converters.SequencingDataModelConverter();
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

            ProcessDefaultModelSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation($"Processing of SSM data submission completed in {Math.Round(stopwatch.Elapsed.TotalSeconds, 2)}s");

            return true;
        });
    }

    private void ProcessDefaultModelSubmission(string submissionId)
    {
        var sequencingData = _submissionService.FindSsmSubmission(submissionId);

        var analysisData = _dataConverter.Convert(sequencingData);

        _dataWriter.SaveData(analysisData, out var audit);

        _annotationTaskService.PopulateTasks(audit.Mutations);

        _submissionService.DeleteSsmSubmission(submissionId);

        _logger.LogInformation(audit.ToString());
    }
}
