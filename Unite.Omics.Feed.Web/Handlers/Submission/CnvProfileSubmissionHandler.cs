using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Feed.Data.Models;
using Unite.Omics.Feed.Data.Writers.CnvProfile;
using Unite.Omics.Feed.Web.Submissions.Repositories.Dna;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class CnvProfileSubmissionHandler(HandlerPriority priority,
    CnvProfileWriter dataWriter,
    TasksProcessingService tasksProcessingService,
    CnvProfileSubmissionRepository submissionRepository,
    ILogger<CnvProfileSubmissionHandler> logger) : SubmissionHandler(priority)
{
    public override void Handle()
    {
        ProcessSubmissionTasks();
    }
    
    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        tasksProcessingService.Process(SubmissionTaskType.DNA_CNV, TaskStatusType.Prepared, 1, (tasks) =>
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
        //TODO: create a converter from submission data to domain data(SampleModel)
        //var convertedData = _converter.Convert(submittedData);
        var convertedData = new SampleModel();

        dataWriter.SaveData(convertedData, out var audit);
        //TODO: create indexing tasks for CNV profiles
        //indexingTaskService.PopulateTasks(audit.CnvsEntries.Except(audit.Cnvs));
        submissionRepository.Delete(submissionId);

        logger.LogInformation("{audit}", audit.ToString());
    }
}