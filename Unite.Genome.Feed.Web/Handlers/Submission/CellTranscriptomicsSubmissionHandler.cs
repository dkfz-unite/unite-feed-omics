using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Feed.Data.Writers.Transcriptomics;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class CellTranscriptomicsSubmissionHandler
{
    private readonly CellDataWriter _dataWriter;
    private readonly TranscriptomicsSubmissionService _submissionService;
    private readonly SampleIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Transcriptomics.Converters.SequencingDataModelConverter _converter;


    public CellTranscriptomicsSubmissionHandler(
        CellDataWriter dataWriter,
        TranscriptomicsSubmissionService submissionService,
        SampleIndexingTaskService indexingTaskService,
        TasksProcessingService tasksProcessingService,
        ILogger<CellTranscriptomicsSubmissionHandler> logger)
    {
        _dataWriter = dataWriter;
        _submissionService = submissionService;
        _indexingTaskService = indexingTaskService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;

        _converter = new Models.Transcriptomics.Converters.SequencingDataModelConverter();
    }


    public void Handle()
    {
        ProcessSubmissionTasks();
    }


    private void ProcessSubmissionTasks()
    {
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.CGE, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed single-cell transcriptomics data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submitedSequencingData = _submissionService.FindBulkSubmission(submissionId);
        var convertedSequencingData = _converter.Convert(submitedSequencingData);

        _dataWriter.SaveData(convertedSequencingData, out var audit);
        _indexingTaskService.PopulateTasks(audit.Samples);
        _submissionService.DeleteCellSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }
}
