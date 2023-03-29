using System.Diagnostics;
using System.Text.Json;
using Unite.Cache.Configuration.Options;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Data.Services.Tasks;
using Unite.Genome.Feed.Data;
using Unite.Genome.Feed.Web.Models.Variants.CNV;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class CopyNumberVariantsSubmissionHandler
{
    private readonly ISqlOptions _sqlOptions;
    private readonly IMongoOptions _mongoOptions;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Variants.CNV.Converters.SequencingDataModelConverter _dataModelConverter;
    private readonly Models.Variants.CNV.Converters.SequencingDataAceSeqModelConverter _dataAceSeqModelConverter;


    public CopyNumberVariantsSubmissionHandler(
        ISqlOptions sqlOptions,
        IMongoOptions mongoOptions,
        TasksProcessingService tasksProcessingService,
        ILogger<CopyNumberVariantsSubmissionHandler> logger)
    {
        _sqlOptions = sqlOptions;
        _mongoOptions = mongoOptions;
        _taskProcessingService = tasksProcessingService;
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
        using var dbContext = new DomainDbContext(_sqlOptions);

        var dataWriter = new SequencingDataWriter(dbContext);
        var annotationTaskService = new CopyNumberVariantAnnotationTaskService(dbContext);
        var submissionService = new VariantsSubmissionService(_mongoOptions);

        var sequencingData = submissionService.FindCnvSubmission(submissionId);
        var analysisData = _dataModelConverter.Convert(sequencingData);

        dataWriter.SaveData(analysisData, out var audit);
        annotationTaskService.PopulateTasks(audit.CopyNumberVariants);
        submissionService.DeleteCnvSubmission(submissionId);

        _logger.LogInformation(audit.ToString());
    }

    private void ProcessAceSeqModelSubmission(string submissionId)
    {
        using var dbContext = new DomainDbContext(_sqlOptions);

        var dataWriter = new SequencingDataWriter(dbContext);
        var annotationTaskService = new CopyNumberVariantAnnotationTaskService(dbContext);
        var submissionService = new VariantsSubmissionService(_mongoOptions);

        var sequencingData = submissionService.FindCnvAceSeqSubmission(submissionId);
        var analysisData = _dataAceSeqModelConverter.Convert(sequencingData);

        dataWriter.SaveData(analysisData, out var audit);
        annotationTaskService.PopulateTasks(audit.CopyNumberVariants);
        submissionService.DeleteCnvAceSeqSubmission(submissionId);

        _logger.LogInformation(audit.ToString());
    }
}
