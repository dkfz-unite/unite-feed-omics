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

public class DnaCnvSubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly CnvAnnotationTaskService _annotationTaskService;
    private readonly CnvIndexingTaskService _indexingTaskService;
    private readonly CnvSubmissionRepository _submissionRepository;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Dna.Cnv.Converters.AnalysisModelConverter _converter;


    public DnaCnvSubmissionHandler(
        AnalysisWriter dataWriter,
        CnvAnnotationTaskService annotationTaskService,
        CnvIndexingTaskService indexingTaskService,
        TasksProcessingService tasksProcessingService,
        CnvSubmissionRepository submissionRepository,
        ILogger<DnaCnvSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _indexingTaskService = indexingTaskService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;
        _submissionRepository = submissionRepository;

        _converter = new Models.Dna.Cnv.Converters.AnalysisModelConverter();
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }


    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.DNA_CNV, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed CNVs data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionRepository.Find<AnalysisModel<Models.Dna.Cnv.VariantModel>>(submissionId)?.Document;
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _annotationTaskService.PopulateTasks(audit.Cnvs);
        _indexingTaskService.PopulateTasks(audit.CnvsEntries.Except(audit.Cnvs));
        _submissionRepository.Delete<AnalysisModel<Models.Dna.Cnv.VariantModel>>(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
