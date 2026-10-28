using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers.Dna;
using Unite.Omics.Feed.Web.Models.Dna.Cnv.Converters;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class DnaCnvpSubmissionHandler : SubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly TasksProcessingService _tasksProcessingService;
    private readonly CnvpSubmissionRepository _submissionRepository;
    private readonly CnvpIndexingTaskService _indexingTaskService;
    private readonly ILogger<DnaCnvpSubmissionHandler> _logger;

    private readonly ProfileModelConverter _converter = new();


    public DnaCnvpSubmissionHandler(
        AnalysisWriter dataWriter,
        TasksProcessingService tasksProcessingService,
        CnvpSubmissionRepository submissionRepository,
        CnvpIndexingTaskService indexingTaskService,
        ILogger<DnaCnvpSubmissionHandler> logger) : base()
    {
        _dataWriter = dataWriter;
        _tasksProcessingService = tasksProcessingService;
        _submissionRepository = submissionRepository;
        _indexingTaskService = indexingTaskService;
        _logger = logger;
    }


    public override void Handle()
    {
        var stopwatch = new Stopwatch();

        _tasksProcessingService.Process(SubmissionTaskType.DNA_CNVP, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed CNVPs data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }
    

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionRepository.Find(submissionId)?.Document;
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        
        _indexingTaskService.PopulateTasks(audit.Cnvps);
        
        _submissionRepository.Delete(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
