using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Genome.Annotations.Services.Rna;
using Unite.Genome.Feed.Data.Writers.Rna;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class BulkRnaSubmissionHandler
{
    private readonly BulkExpDataWriter _dataWriter;
    private readonly ExpressionsAnnotationService _annotationService;
    private readonly RnaSubmissionService _submissionService;
    private readonly GeneIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Rna.Converters.SeqDataModelConverter _converter;


	public BulkRnaSubmissionHandler(
        BulkExpDataWriter dataWriter,
        ExpressionsAnnotationService annotationService,
        RnaSubmissionService submissionService,
        GeneIndexingTaskService indexingTaskService,
        TasksProcessingService tasksProcessingService,
        ILogger<BulkRnaSubmissionHandler> logger)
	{
        _dataWriter = dataWriter;
        _annotationService = annotationService;
        _submissionService = submissionService;
        _indexingTaskService = indexingTaskService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;

        _converter = new Models.Rna.Converters.SeqDataModelConverter();
	}


	public void Handle()
	{
		ProcessSubmissionTasks();
	}


	private void ProcessSubmissionTasks()
	{
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.BGE, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed bulk transcriptomics data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }

    private void ProcessSubmission(string submissionId)
    {
        var submitedSequencingData = _submissionService.FindBulkSubmission(submissionId);
        var annotatedExpressions = AnnotateExpressions(_annotationService, submitedSequencingData.Entries);
        var convertedExpressions = Convert(annotatedExpressions).ToArray();
        var convertedSequencingData = _converter.Convert(submitedSequencingData);
        convertedSequencingData.Exps = convertedExpressions;

        _dataWriter.SaveData(convertedSequencingData, out var audit);
        _indexingTaskService.PopulateTasks(audit.Genes);
        _submissionService.DeleteBulkSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }

	private Annotations.Services.Models.Rna.GeneExpressionModel[] AnnotateExpressions(ExpressionsAnnotationService annotationService, Models.Rna.BulkExpressionModel[] expressions)
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

    private static IEnumerable<Data.Models.Rna.GeneExpressionModel> Convert(Annotations.Services.Models.Rna.GeneExpressionModel[] source)
    {
        foreach (var model in source)
        {
            yield return new Data.Models.Rna.GeneExpressionModel
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
