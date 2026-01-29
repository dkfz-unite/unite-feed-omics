using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers.Dna;
using Unite.Omics.Feed.Web.Models.Base;
using Unite.Omics.Feed.Web.Services.Annotation;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class DnaSvSubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly SvAnnotationTaskService _annotationTaskService;
    private readonly SvIndexingTaskService _indexingTaskService;
    private readonly SvSubmissionRepository _submissionRepository;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Dna.Sv.Converters.AnalysisModelConverter _converter;


    public DnaSvSubmissionHandler(
        AnalysisWriter dataWriter,
        SvAnnotationTaskService annotationTaskService,
        SvIndexingTaskService indexingTaskService,
        DnaSubmissionService submissionService,
        TasksProcessingService taskProcessingService,
        SvSubmissionRepository submissionRepository,
        ILogger<DnaSvSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _indexingTaskService = indexingTaskService;
        _taskProcessingService = taskProcessingService;
        _logger = logger;
        _submissionRepository = submissionRepository;

        _converter = new Models.Dna.Sv.Converters.AnalysisModelConverter();
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }


    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.DNA_SV, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed SVs data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionRepository.Find<AnalysisModel<Models.Dna.Sv.VariantModel>>(submissionId)?.Document;
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _annotationTaskService.PopulateTasks(audit.Svs);
        _indexingTaskService.PopulateTasks(audit.SvsEntries.Except(audit.Svs));
        _submissionRepository.Delete<AnalysisModel<Models.Dna.Sv.VariantModel>>(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
