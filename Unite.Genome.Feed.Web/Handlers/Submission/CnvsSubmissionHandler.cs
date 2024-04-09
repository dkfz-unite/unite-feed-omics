using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Data.Writers.Variants;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class CnvsSubmissionHandler
{
    private readonly VariantsDataWriter _dataWriter;
    private readonly CnvAnnotationTaskService _annotationTaskService;
    private readonly VariantsSubmissionService _submissionService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Variants.CNV.Converters.SequencingDataModelConverter _converter;


    public CnvsSubmissionHandler(
        VariantsDataWriter dataWriter,
        CnvAnnotationTaskService annotationTaskService,
        VariantsSubmissionService submissionService,
        TasksProcessingService tasksProcessingService,
        ILogger<CnvsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _submissionService = submissionService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;

        _converter = new Models.Variants.CNV.Converters.SequencingDataModelConverter();
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }


    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.CNV, 1, (tasks) =>
        {
            _logger.LogInformation($"Processing CNV data submission");

            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processing of CNV data submission completed in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedSequencingData = _submissionService.FindCnvSubmission(submissionId);
        var convertedSequencingData = _converter.Convert(submittedSequencingData);

        _dataWriter.SaveData(convertedSequencingData, out var audit);
        _annotationTaskService.PopulateTasks(audit.Cnvs);
        _submissionService.DeleteCnvSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
