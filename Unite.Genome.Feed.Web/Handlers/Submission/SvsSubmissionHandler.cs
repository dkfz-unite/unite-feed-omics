using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Data.Writers.Dna;
using Unite.Genome.Feed.Web.Services.Annotation;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class SvsSubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly SvAnnotationTaskService _annotationTaskService;
    private readonly SvIndexingTaskService _indexingTaskService;
    private readonly DnaSubmissionService _submissionService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Dna.Sv.Converters.AnalysisModelConverter _converter;


    public SvsSubmissionHandler(
        AnalysisWriter dataWriter,
        SvAnnotationTaskService annotationTaskService,
        SvIndexingTaskService indexingTaskService,
        DnaSubmissionService submissionService,
        TasksProcessingService taskProcessingService,
        ILogger<SvsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _annotationTaskService = annotationTaskService;
        _indexingTaskService = indexingTaskService;
        _submissionService = submissionService;
        _taskProcessingService = taskProcessingService;
        _logger = logger;

        _converter = new Models.Dna.Sv.Converters.AnalysisModelConverter();
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
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed SVs data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionService.FindSvSubmission(submissionId);
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        _annotationTaskService.PopulateTasks(audit.Svs);
        _indexingTaskService.PopulateTasks(audit.SvsEntries.Except(audit.Svs));
        _submissionService.DeleteSvSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
