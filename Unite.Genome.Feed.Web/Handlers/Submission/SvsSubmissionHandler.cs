using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Data.Writers.Variants;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class SvsSubmissionHandler
{
    private readonly VariantsDataWriter _dataWriter;
    private readonly SvAnnotationTaskService _annotationTaskService;
    private readonly VariantsSubmissionService _submissionService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Variants.SV.Converters.SequencingDataModelConverter _converter;


    public SvsSubmissionHandler(
        VariantsDataWriter dataWriter,
        SvAnnotationTaskService annotationTaskService,
        VariantsSubmissionService submissionService,
        TasksProcessingService taskProcessingService,
        ILogger<SvsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _submissionService = submissionService;
        _taskProcessingService = taskProcessingService;
        _logger = logger;

        _converter = new Models.Variants.SV.Converters.SequencingDataModelConverter();
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }


    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.SV, 1, (tasks) =>
        {
            _logger.LogInformation($"Processing SV data submission");

            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processing of SV data submission completed in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedSequencingData = _submissionService.FindSvSubmission(submissionId);
        var convertedSequencingData = _converter.Convert(submittedSequencingData);

        _dataWriter.SaveData(convertedSequencingData, out var audit);
        _annotationTaskService.PopulateTasks(audit.Svs);
        _submissionService.DeleteSvSubmission(submissionId);

        _dataWriter.Refresh();
        _logger.LogInformation("{audit}", audit.ToString());
    }
}
