using System.Diagnostics;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Genome.Feed.Data;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class StructuralVariantsSubmissionHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly VariantsSubmissionService _submissionService;
    private readonly SequencingDataWriter _dataWriter;
    private readonly StructuralVariantAnnotationTaskService _annotationTaskService;
    private readonly ILogger _logger;

    private readonly Models.Variants.SV.Converters.SequencingDataModelConverter _dataConverter;


    public StructuralVariantsSubmissionHandler(
        TasksProcessingService taskProcessingService,
        VariantsSubmissionService submissionService,
        SequencingDataWriter dataWriter,
        StructuralVariantAnnotationTaskService annotationTaskService,
        ILogger<StructuralVariantsSubmissionHandler> logger)
    {
        _taskProcessingService = taskProcessingService;
        _submissionService = submissionService;
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _logger = logger;

        _dataConverter = new Models.Variants.SV.Converters.SequencingDataModelConverter();
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

            ProcessDefaultModelSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation($"Processing of SV data submission completed in {Math.Round(stopwatch.Elapsed.TotalSeconds, 2)}s");

            return true;
        });
    }

    private void ProcessDefaultModelSubmission(string submissionId)
    {
        var sequencingData = _submissionService.FindSvSubmission(submissionId);

        var analysisData = _dataConverter.Convert(sequencingData);

        _dataWriter.SaveData(analysisData, out var audit);

        _annotationTaskService.PopulateTasks(audit.StructuralVariants);

        _submissionService.DeleteSvSubmission(submissionId);

        _logger.LogInformation(audit.ToString());
    }
}
