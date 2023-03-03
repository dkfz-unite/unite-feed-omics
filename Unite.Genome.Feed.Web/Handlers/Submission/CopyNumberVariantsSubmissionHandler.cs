using System.Diagnostics;
using System.Text.Json;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Genome.Feed.Data;
using Unite.Genome.Feed.Web.Models.Variants.CNV;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class CopyNumberVariantsSubmissionHandler
{
    private readonly TasksProcessingService _taskProcessingService;
    private readonly VariantsSubmissionService _submissionService;
    private readonly SequencingDataWriter _dataWriter;
    private readonly CopyNumberVariantAnnotationTaskService _annotationTaskService;
    private readonly ILogger _logger;

    private readonly Models.Variants.CNV.Converters.SequencingDataModelConverter _dataModelConverter;
    private readonly Models.Variants.CNV.Converters.SequencingDataAceSeqModelConverter _dataAceSeqModelConverter;


    public CopyNumberVariantsSubmissionHandler(
        TasksProcessingService tasksProcessingService,
        VariantsSubmissionService submissionService,
        SequencingDataWriter dataWriter,
        CopyNumberVariantAnnotationTaskService annotationTaskService,
        ILogger<CopyNumberVariantsSubmissionHandler> logger)
    {
        _taskProcessingService = tasksProcessingService;
        _submissionService = submissionService;
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _logger = logger;

        _dataModelConverter = new Models.Variants.CNV.Converters.SequencingDataModelConverter();
        _dataAceSeqModelConverter = new Models.Variants.CNV.Converters.SequencingDataAceSeqModelConverter();
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

            var target = tasks[0].Target;

            var data = GetSubmissionData(tasks[0].Data);

            if (data?.Type == SubmissionType.Default)
            {
                ProcessDefaultModelSubmission(target);
            }
            else if (data?.Type == SubmissionType.AceSeq)
            {
                ProcessAceSeqModelSubmission(target);
            }

            stopwatch.Stop();

            _logger.LogInformation($"Processing of CNV data submission completed in {Math.Round(stopwatch.Elapsed.TotalSeconds, 2)}s");

            return true;
        });
    }

    private SubmissionData GetSubmissionData(string submissionData)
    {
        try
        {
            return JsonSerializer.Deserialize<SubmissionData>(submissionData);
        }
        catch
        {
            return null;
        }
    }

    private void ProcessDefaultModelSubmission(string submissionId)
    {
        var sequencingData = _submissionService.FindCnvSubmission(submissionId);

        var analysisData = _dataModelConverter.Convert(sequencingData);

        _dataWriter.SaveData(analysisData, out var audit);

        _annotationTaskService.PopulateTasks(audit.CopyNumberVariants);

        _submissionService.DeleteCnvSubmission(submissionId);

        _logger.LogInformation(audit.ToString());
    }

    private void ProcessAceSeqModelSubmission(string submissionId)
    {
        var sequencingData = _submissionService.FindCnvAceSeqSubmission(submissionId);

        var analysisData = _dataAceSeqModelConverter.Convert(sequencingData);

        _dataWriter.SaveData(analysisData, out var audit);

        _annotationTaskService.PopulateTasks(audit.CopyNumberVariants);

        _submissionService.DeleteCnvAceSeqSubmission(submissionId);

        _logger.LogInformation(audit.ToString());
    }
}
