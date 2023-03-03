using System.Diagnostics;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services.Tasks;
using Unite.Genome.Annotations.Services.Models.Transcriptomics;
using Unite.Genome.Annotations.Services.Transcriptomics;
using Unite.Genome.Feed.Data;
using Unite.Genome.Feed.Web.Models.Transcriptomics;
using Unite.Genome.Feed.Web.Models.Transcriptomics.Converters;
using Unite.Genome.Feed.Web.Services.Indexing;
using Unite.Genome.Feed.Web.Submissions;

namespace Unite.Genome.Feed.Web.Handlers.Submission;

public class TranscriptomicsSubmissionHandler
{
    private readonly TranscriptomicsSubmissionService _submissionService;
    private readonly TranscriptomicsAnnotationService _annotationService;
    private readonly TranscriptomicsDataWriter _dataWriter;
    private readonly TranscriptomicsDataModelConverter _dataConverter;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly GeneIndexingTaskService _geneIndexingTaskService;
    private ILogger _logger;


	public TranscriptomicsSubmissionHandler(
        TranscriptomicsSubmissionService submissionService,
        TranscriptomicsAnnotationService annotationService,
        TranscriptomicsDataWriter dataWriter,
        TasksProcessingService tasksProcessingService,
        GeneIndexingTaskService geneIndexingTaskService,
        ILogger<TranscriptomicsSubmissionHandler> logger)
	{
        _submissionService = submissionService;
        _annotationService = annotationService;
        _dataWriter = dataWriter;
        _dataConverter = new TranscriptomicsDataModelConverter();
        _taskProcessingService = tasksProcessingService;
        _geneIndexingTaskService = geneIndexingTaskService;
        _logger = logger;
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
            _logger.LogInformation($"Processing transcriptomics data submission");

            stopwatch.Restart();

            var submission = _submissionService.FindSubmission(tasks[0].Target);

            var expressionModels = AnnotateExpressions(submission.Expressions);

            var expressionData = Convert(expressionModels).ToArray();

            var analysisData = _dataConverter.Convert(submission);

            analysisData.AnalysedSamples.First().GeneExpressions = expressionData;

            _dataWriter.SaveData(analysisData, out var audit);

            _geneIndexingTaskService.PopulateTasks(audit.Genes);

            _submissionService.DeleteSubmission(tasks[0].Target);

            _logger.LogInformation(audit.ToString());

            stopwatch.Stop();

            _logger.LogInformation($"Processing of transcriptomics data submission completed in {Math.Round(stopwatch.Elapsed.TotalSeconds, 2)}s");

            return true;
        });
    }

	private GeneExpressionModel[] AnnotateExpressions(ExpressionModel[] expressions)
	{
        var dataType = expressions.First().GetDataType();

        var data = expressions
            .Select(expression => expression.GetData())
            .DistinctBy(expression => expression.Key)
            .ToDictionary(expression => expression.Key, expression => expression.Value);

        return dataType switch
        {
            1 => _annotationService.AnnotateByGeneId(data),
            2 => _annotationService.AnnotateByGeneSymbol(data),
            3 => _annotationService.AnnotateByTranscriptId(data),
            4 => _annotationService.AnnotateByTranscriptSymbol(data),
            _ => Array.Empty<GeneExpressionModel>()
        };
    }

    private IEnumerable<Data.Models.Transcriptomics.GeneExpressionModel> Convert(GeneExpressionModel[] source)
    {
        foreach (var model in source)
        {
            yield return new Data.Models.Transcriptomics.GeneExpressionModel
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
