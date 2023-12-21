using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Annotations.Services.Transcriptomics;
using Unite.Genome.Feed.Data.Writers.Transcriptomics;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class TranscriptomicsSubmissionHandler
{
    private readonly BulkExpressionsDataWriter _dataWriter;
    private readonly BulkExpressionsAnnotationService _annotationService;
    private readonly TranscriptomicsSubmissionService _submissionService;
    private readonly GeneIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Transcriptomics.Converters.SequencingDataModelConverter _converter;


	public TranscriptomicsSubmissionHandler(
        BulkExpressionsDataWriter dataWriter,
        BulkExpressionsAnnotationService annotationService,
        TranscriptomicsSubmissionService submissionService,
        GeneIndexingTaskService indexingTaskService,
        TasksProcessingService tasksProcessingService,
        ILogger<TranscriptomicsSubmissionHandler> logger)
	{
        _dataWriter = dataWriter;
        _annotationService = annotationService;
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

        _taskProcessingService.Process(SubmissionTaskType.TEX, 1, (tasks) =>
        {
            _logger.LogInformation($"Processing bulk transcriptomics data submission");

            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processing of bulk transcriptomics data submission completed in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submitedSequencingData = _submissionService.FindSubmission(submissionId);
        var annotatedExpressions = AnnotateExpressions(_annotationService, submitedSequencingData.Entries);
        var convertedExpressions = Convert(annotatedExpressions).ToArray();
        var convertedSequencingData = _converter.Convert(submitedSequencingData);
        convertedSequencingData.BulkExpressions = convertedExpressions;

        _dataWriter.SaveData(convertedSequencingData, out var audit);
        _indexingTaskService.PopulateTasks(audit.Genes);
        _submissionService.DeleteSubmission(submissionId);

        _dataWriter.Refresh();
        _logger.LogInformation("{audit}", audit.ToString());
    }

	private Annotations.Services.Models.Transcriptomics.BulkExpressionModel[] AnnotateExpressions(BulkExpressionsAnnotationService annotationService, Models.Transcriptomics.BulkExpressionModel[] expressions)
	{
        var dataType = expressions.First().GetDataType();

        var data = expressions
            .Select(expression => expression.GetData())
            .DistinctBy(expression => expression.Key)
            .ToDictionary(expression => expression.Key, expression => expression.Value);

        return dataType switch
        {
            1 => annotationService.AnnotateByGeneId(data),
            2 => annotationService.AnnotateByGeneSymbol(data),
            3 => annotationService.AnnotateByTranscriptId(data),
            4 => annotationService.AnnotateByTranscriptSymbol(data),
            _ => []
        };
    }

    private static IEnumerable<Data.Models.Transcriptomics.BulkExpressionModel> Convert(Annotations.Services.Models.Transcriptomics.BulkExpressionModel[] source)
    {
        foreach (var model in source)
        {
            yield return new Data.Models.Transcriptomics.BulkExpressionModel
            {
                Gene = new Data.Models.GeneModel
                {
                    Id = model.Gene.Id,
                    Symbol = model.Gene.Symbol,
                    Description = model.Gene.Description,
                    Biotype = model.Gene.Biotype,
                    Chromosome = model.Gene.Chromosome,
                    Start = model.Gene.Start,
                    End = model.Gene.End,
                    Strand = model.Gene.Strand,
                    ExonicLength = model.Gene.ExonicLength
                },

                Reads = model.Reads,
                TPM = model.TPM,
                FPKM = model.FPKM
            };
        }
    }
}
