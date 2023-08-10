using System.Diagnostics;
using Unite.Cache.Configuration.Options;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Data.Services;
using Unite.Data.Services.Configuration.Options;
using Unite.Data.Services.Tasks;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
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
    private readonly ISqlOptions _sqlOptions;
    private readonly IEnsemblDataOptions _ensemblOptions;
    private readonly IMongoOptions _mongoOptions;
    private readonly TranscriptomicsDataModelConverter _dataConverter;
    private readonly TasksProcessingService _taskProcessingService;
    private ILogger _logger;


	public TranscriptomicsSubmissionHandler(
        ISqlOptions sqlOptions,
        IEnsemblDataOptions ensemblOptions,
        IMongoOptions mongoOptions,
        TranscriptomicsSubmissionService submissionService,
        TranscriptomicsAnnotationService annotationService,
        TranscriptomicsDataWriter dataWriter,
        TasksProcessingService tasksProcessingService,
        GeneIndexingTaskService geneIndexingTaskService,
        ILogger<TranscriptomicsSubmissionHandler> logger)
	{
        _sqlOptions = sqlOptions;
        _ensemblOptions = ensemblOptions;
        _mongoOptions = mongoOptions;
        _dataConverter = new TranscriptomicsDataModelConverter();
        _taskProcessingService = tasksProcessingService;
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

            ProcessSubmissionTasks(tasks);

            stopwatch.Stop();

            _logger.LogInformation($"Processing of transcriptomics data submission completed in {Math.Round(stopwatch.Elapsed.TotalSeconds, 2)}s");

            return true;
        });
    }

    private void ProcessSubmissionTasks(Unite.Data.Entities.Tasks.Task[] tasks)
    {
        using var dbContext = new DomainDbContext(_sqlOptions);

        var submissionService = new TranscriptomicsSubmissionService(_mongoOptions);
        var annotationService = new TranscriptomicsAnnotationService(_ensemblOptions, dbContext);
        var dataWriter = new TranscriptomicsDataWriter(dbContext);
        var indexingTaskService = new GeneIndexingTaskService(dbContext);

        var submission = submissionService.FindSubmission(tasks[0].Target);
        var expressionModels = AnnotateExpressions(annotationService, submission.Expressions);
        var expressionData = Convert(expressionModels).ToArray();
        var analysisData = _dataConverter.Convert(submission);
        analysisData.AnalysedSamples.First().GeneExpressions = expressionData;

        dataWriter.SaveData(analysisData, out var audit);
        indexingTaskService.PopulateTasks(audit.Genes);
        submissionService.DeleteSubmission(tasks[0].Target);

        _logger.LogInformation(audit.ToString());
    }

	private GeneExpressionModel[] AnnotateExpressions(TranscriptomicsAnnotationService annotationService, ExpressionModel[] expressions)
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
