using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers.Dna;
using Unite.Omics.Feed.Web.Models.Dna.CnvProfile.Converters;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class CnvProfileSubmissionHandler : SubmissionHandler
{
    private readonly CnvProfileModelConverter _converter = new();
    private readonly AnalysisWriter _dataWriter;
    private readonly TasksProcessingService _tasksProcessingService;
    private readonly CnvProfileSubmissionRepository _submissionRepository;
    private readonly CnvProfileIndexingTaskService _indexingTaskService;
    private readonly ILogger<CnvProfileSubmissionHandler> _logger;

    public CnvProfileSubmissionHandler(HandlerPriority priority,
        AnalysisWriter dataWriter,
        TasksProcessingService tasksProcessingService,
        CnvProfileSubmissionRepository submissionRepository,
        CnvProfileIndexingTaskService indexingTaskService,
        ILogger<CnvProfileSubmissionHandler> logger) : base(priority)
    {
        _dataWriter = dataWriter;
        _tasksProcessingService = tasksProcessingService;
        _submissionRepository = submissionRepository;
        _indexingTaskService = indexingTaskService;
        _logger = logger;
    }

    public override Task Handle()
    {
        return Task.Run(ProcessSubmissionTasks);
    }
    
    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _tasksProcessingService.Process(SubmissionTaskType.DNA_CNV_PROFILE, TaskStatusType.Prepared, 1, (tasks) =>
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
        var submittedData = _submissionRepository.Find(submissionId)?.Document;
        var convertedData = _converter.Convert(submittedData);

        _dataWriter.SaveData(convertedData, out var audit);
        
        var allIds = new HashSet<int>(audit.CnvProfilesCreated);
        allIds.UnionWith(audit.CnvProfilesUpdated);
        _indexingTaskService.PopulateTasks(allIds);
        
        _submissionRepository.Delete(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}