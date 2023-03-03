using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Audit;
using Unite.Genome.Feed.Data.Extensions;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Models.Transcriptomics;
using Unite.Genome.Feed.Data.Repositories;
using Unite.Genome.Feed.Data.Repositories.Transcriptomics;

namespace Unite.Genome.Feed.Data;

public class TranscriptomicsDataWriter : DataWriter<AnalysisModel, TranscriptomicsDataUploadAudit>
{
    private const int _batchSize = 1000;

    private readonly AnalysisRepository _analysisRepository;
    private readonly AnalysedSampleRepository _analysedSampleRepository;
    private readonly GeneRepository _geneRepository;
    private readonly GeneExpressionRepository _geneExpressionRepository;


    public TranscriptomicsDataWriter(DomainDbContext dbContext) : base(dbContext)
    {
        _analysisRepository = new AnalysisRepository(dbContext);
        _analysedSampleRepository = new AnalysedSampleRepository(dbContext);
        _geneRepository = new GeneRepository(dbContext);
        _geneExpressionRepository = new GeneExpressionRepository(dbContext);
    }


    protected override void ProcessModel(AnalysisModel analysisModel, ref TranscriptomicsDataUploadAudit audit)
    {
        var analysis = _analysisRepository.FindOrCreate(analysisModel);

        foreach (var analysedSampleModel in analysisModel.AnalysedSamples)
        {
            var analysedSample = _analysedSampleRepository.FindOrCreate(analysis.Id, analysedSampleModel);

            if (analysedSampleModel.GeneExpressions != null)
            {
                WriteGeneExpressions(analysedSample.Id, analysedSampleModel.GeneExpressions, ref audit);
            }
        }
    }

    private void WriteGeneExpressions(int analysedSampleId, IEnumerable<GeneExpressionModel> geneExpressionModels, ref TranscriptomicsDataUploadAudit audit)
    {
        var queue = new Queue<GeneExpressionModel>(geneExpressionModels);

        _geneExpressionRepository.RemoveAll(analysedSampleId);

        while (queue.Any())
        {
            var chunk = queue.Dequeue(_batchSize).ToArray();

            var existingGenes = _geneRepository.Find(chunk.Select(model => model.Gene)).ToArray();

            var createdGenes = _geneRepository.CreateMissing(chunk.Select(model => model.Gene), existingGenes).ToArray();

            var genesCache = Enumerable.Concat(createdGenes, existingGenes).ToArray();


            var geneExpressions = _geneExpressionRepository.CreateAll(analysedSampleId, chunk, genesCache).ToArray();


            audit.GenesCreated += createdGenes.Count();

            audit.ExpressionsAssociated += geneExpressions.Count();

            audit.Genes.AddRange(geneExpressions.Select(entity => entity.GeneId));
        }
    }
}
