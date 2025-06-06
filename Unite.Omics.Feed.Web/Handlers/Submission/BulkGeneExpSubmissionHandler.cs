using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Annotations.Services.Rna;
using Unite.Omics.Feed.Data.Writers.Rna;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class BulkGeneExpSubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly ExpressionsAnnotationService _annotationService;
    private readonly RnaSubmissionService _submissionService;
    private readonly GeneIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Rna.Converters.AnalysisModelConverter _converter;


	public BulkGeneExpSubmissionHandler(
        AnalysisWriter dataWriter,
        ExpressionsAnnotationService annotationService,
        RnaSubmissionService submissionService,
        GeneIndexingTaskService indexingTaskService,
        TasksProcessingService tasksProcessingService,
        ILogger<BulkGeneExpSubmissionHandler> logger)
	{
        _dataWriter = dataWriter;
        _annotationService = annotationService;
        _submissionService = submissionService;
        _indexingTaskService = indexingTaskService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;

        _converter = new Models.Rna.Converters.AnalysisModelConverter();
	}


	public void Handle()
	{
		ProcessSubmissionTasks();
	}


	private void ProcessSubmissionTasks()
	{
        var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.RNA_EXP, TaskStatusType.Prepared, 1, (tasks) =>
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
        var submittedData = _submissionService.FindExpSubmission(submissionId);
        var annotatedExpressions = AnnotateExpressions(_annotationService, submittedData.Entries);
        var convertedExpressions = Convert(annotatedExpressions).ToArray();
        var convertedData = _converter.Convert(submittedData);
        convertedData.Exps = convertedExpressions;

        _dataWriter.SaveData(convertedData, out var audit);
        _indexingTaskService.PopulateTasks(audit.Genes);
        _submissionService.DeleteExpSubmission(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }

	private Annotations.Services.Models.Rna.GeneExpressionModel[] AnnotateExpressions(ExpressionsAnnotationService annotationService, Models.Rna.ExpressionModel[] expressions)
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
