using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Writers.Dna;
using Unite.Omics.Feed.Web.Models.Dna.CnvProfile.Converters;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class CnvProfileSubmissionHandler(HandlerPriority priority,
    AnalysisWriter dataWriter,
    TasksProcessingService tasksProcessingService,
    CnvProfileSubmissionRepository submissionRepository,
    CnvProfileIndexingTaskService indexingTaskService,
    ILogger<CnvProfileSubmissionHandler> logger) : SubmissionHandler(priority)
{
    private readonly CnvProfileModelConverter _converter = new();

    public override Task Handle()
    {
        return Task.Run(ProcessSubmissionTasks);
    }
    
    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        tasksProcessingService.Process(SubmissionTaskType.DNA_CNV_PROFILE, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            logger.LogInformation("Processed CNVs data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submittedData = submissionRepository.Find(submissionId)?.Document;
        var convertedData = _converter.Convert(submittedData);

        dataWriter.SaveData(convertedData, out var audit);
        
        var allIds = new HashSet<int>(audit.CnvProfilesCreated);
        allIds.UnionWith(audit.CnvProfilesUpdated);
        indexingTaskService.PopulateTasks(allIds);
        
        submissionRepository.Delete(submissionId);

        logger.LogInformation("{audit}", audit.ToString());
    }
}