using System.Diagnostics;
using Unite.Data.Context.Services.Tasks;
using Unite.Data.Entities.Tasks.Enums;
using Unite.Omics.Annotations.Services.Prot;
using Unite.Omics.Feed.Data.Writers.Prot;
using Unite.Omics.Feed.Web.Services.Indexing;
using Unite.Omics.Feed.Web.Submissions.Repositories.Prot;

namespace Unite.Omics.Feed.Web.Handlers.Submission;

public class ProtExpSubmissionHandler : SubmissionHandler
{
    private readonly AnalysisWriter _dataWriter;
    private readonly ExpressionsAnnotationService _annotationService;
    private readonly ExpressionSubmissionRepository _submissionRepository;
    private readonly ProteinIndexingTaskService _indexingTaskService;
    private readonly TasksProcessingService _taskProcessingService;
    private readonly ILogger _logger;

    private readonly Models.Prot.Converters.AnalysisModelConverter _converter;


    public ProtExpSubmissionHandler(
        HandlerPriority priority,
        AnalysisWriter dataWriter,
        ExpressionsAnnotationService annotationService,
        ProteinIndexingTaskService indexingTaskService,
        TasksProcessingService tasksProcessingService,
        ExpressionSubmissionRepository submissionRepository,
        ILogger<ProtExpSubmissionHandler> logger) : base(priority)
    {
        _dataWriter = dataWriter;
        _annotationService = annotationService;
        _indexingTaskService = indexingTaskService;
        _taskProcessingService = tasksProcessingService;
        _logger = logger;
        _submissionRepository = submissionRepository;

        _converter = new Models.Prot.Converters.AnalysisModelConverter();
    }


    public override void Handle()
    {
        ProcessSubmissionTasks();
    }


    private void ProcessSubmissionTasks()
    {
       var stopwatch = new Stopwatch();

        _taskProcessingService.Process(SubmissionTaskType.PROT_EXP, TaskStatusType.Prepared, 1, (tasks) =>
        {
            stopwatch.Restart();

            ProcessSubmission(tasks[0].Target);

            stopwatch.Stop();

            _logger.LogInformation("Processed proteomics data submission in {time}s", Math.Round(stopwatch.Elapsed.TotalSeconds, 2));

            return true;
        });
    }


    private void ProcessSubmission(string submissionId)
    {
        var submittedData = _submissionRepository.FindDocument(submissionId);
        var annotatedExpressions = AnnotateExpressions(submittedData.Entries);
        var convertedExpressions = Convert(annotatedExpressions).ToArray();
        var convertedData = _converter.Convert(submittedData);
        convertedData.ProteinExpressions = convertedExpressions;

        _dataWriter.SaveData(convertedData, out var audit);
        _indexingTaskService.PopulateTasks(audit.Proteins);
        _submissionRepository.Delete(submissionId);

        _logger.LogInformation("{audit}", audit.ToString());
    }

    private Annotations.Services.Models.Prot.ProteinExpressionModel[] AnnotateExpressions(IEnumerable<Models.Prot.ExpressionModel> expressions)
    {
        var keyType = expressions.First().GetKeyType();

        var data = expressions.Select(expression => expression.GetData())
            .DistinctBy(expression => expression.Key)
            .ToDictionary(expression => expression.Key, expression => expression.Value);

        return keyType switch
        {
            1 => _annotationService.AnnotateByProtreinId(data),
            2 => _annotationService.AnnotateByProteinAccession(data),
            3 => _annotationService.AnnotateByProteinSymbol(data),
            _ => []
        };
    }

    private static IEnumerable<Data.Models.Prot.ProteinExpressionModel> Convert(IEnumerable<Annotations.Services.Models.Prot.ProteinExpressionModel> source)
    {
        foreach (var model in source)
        {
            var expressionModel = new Data.Models.Prot.ProteinExpressionModel
            {
                Raw = model.Raw,
                Normalized = model.Normalized
            };
            
            if (model.Protein != null)
            {
                expressionModel.Protein = new Data.Models.ProteinModel
                {
                    Id = model.Protein.StableId,
                    Accession = model.Protein.Accession,
                    Symbol = model.Protein.Symbol,
                    Description = model.Protein.Description,
                    Database = model.Protein.Database,
                    Chromosome = model.Protein.Chromosome,
                    Start = model.Protein.Start,
                    End = model.Protein.End,
                    Length = model.Protein.Length,
                    IsCanonical = model.Protein.IsCanonical
                };

                if (model.Protein.Transcript != null)
                {
                    expressionModel.Protein.Transcript = new Data.Models.TranscriptModel
                    {
                        Id = model.Protein.Transcript.StableId,
                        Symbol = model.Protein.Transcript.Symbol,
                        Description = model.Protein.Transcript.Description,
                        Biotype = model.Protein.Transcript.Biotype,
                        IsCanonical = model.Protein.Transcript.IsCanonical,
                        Chromosome = model.Protein.Transcript.Chromosome,
                        Start = model.Protein.Transcript.Start,
                        End = model.Protein.Transcript.End,
                        Strand = model.Protein.Transcript.Strand,
                        ExonicLength = model.Protein.Transcript.ExonicLength
                    };

                    if (model.Protein.Transcript.Gene != null)
                    {
                        expressionModel.Protein.Transcript.Gene = new Data.Models.GeneModel
                        {
                            Id = model.Protein.Transcript.Gene.StableId,
                            Symbol = model.Protein.Transcript.Gene.Symbol,
                            Description = model.Protein.Transcript.Gene.Description,
                            Biotype = model.Protein.Transcript.Gene.Biotype,
                            Chromosome = model.Protein.Transcript.Gene.Chromosome,
                            Start = model.Protein.Transcript.Gene.Start,
                            End = model.Protein.Transcript.Gene.End,
                            Strand = model.Protein.Transcript.Gene.Strand,
                            ExonicLength = model.Protein.Transcript.Gene.ExonicLength
                        };
                    }
                }
            }

            yield return expressionModel;
        }
    }
}
