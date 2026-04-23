using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Data.Models;
using Unite.Omics.Feed.Data.Models.Prot;
using Unite.Omics.Feed.Data.Repositories;
using Unite.Omics.Feed.Data.Repositories.Prot;

namespace Unite.Omics.Feed.Data.Writers.Prot;

public class AnalysisWriter : DataWriter<SampleModel, AnalysisWriteAudit>
{
    private const int _batchSize = 1000;

    private readonly IGenomeOptions _genomeOptions;
    private GeneRepository _geneRepository;
    private TranscriptRepository _transcriptRepository;
    private ProteinRepository _proteinRepository;
    private ExpressionRepository _proteinExpressionRepository;

    public AnalysisWriter(IDbContextFactory<DomainDbContext> dbContextFactory, IGenomeOptions genomeOptions) : base(dbContextFactory)
    {
        _genomeOptions = genomeOptions;
    }

    protected override void Initialize(DomainDbContext dbContext)
    {
        _sampleRepository = new SampleRepository(dbContext);
        _geneRepository = new GeneRepository(dbContext, _genomeOptions);
        _transcriptRepository = new TranscriptRepository(dbContext, _genomeOptions);
        _proteinRepository = new ProteinRepository(dbContext, _genomeOptions);
        _proteinExpressionRepository = new ExpressionRepository(dbContext, _genomeOptions);
        _resourceRepository = new ResourceRepository(dbContext);
    }

    protected override void ProcessModel(SampleModel model, ref AnalysisWriteAudit audit)
    {
        var sampleId = WriteSample(model, ref audit);

        if (model.ProteinExpressions.IsNotEmpty())
            WriteExpressions(sampleId, model.ProteinExpressions, ref audit);

        if (model.Resources.IsNotEmpty())
            WriteResources(sampleId, model.Resources, ref audit);
    }

    private void WriteExpressions(int sampleId, IEnumerable<ProteinExpressionModel> models, ref AnalysisWriteAudit audit)
    {
        var queue = new Queue<ProteinExpressionModel>(models);

        _proteinExpressionRepository.RemoveAll(sampleId);

        while (queue.Any())
        {
            var chunk = queue.Dequeue(_batchSize).ToArray();


            var allGenes = chunk.Select(model => model.Protein.Transcript.Gene).DistinctBy(gene => gene.Id).ToArray();

            var existingGenes = _geneRepository.Find(allGenes).ToArray();

            var createdGenes = _geneRepository.CreateMissing(allGenes, existingGenes).ToArray();

            var genesCache = Enumerable.Concat(createdGenes, existingGenes).ToArray();


            var allTranscripts = chunk.Select(model => model.Protein.Transcript).DistinctBy(transcript => transcript.Id).ToArray();

            var existingTranscripts = _transcriptRepository.Find(allTranscripts).ToArray();

            var createdTranscripts = _transcriptRepository.CreateMissing(allTranscripts, existingTranscripts, existingGenes).ToArray();

            var transcriptsCache = Enumerable.Concat(createdTranscripts, existingTranscripts).ToArray();


            var allProteins = chunk.Select(model => model.Protein).DistinctBy(protein => protein.Id).ToArray();

            var existingProteins = _proteinRepository.Find(allProteins).ToArray();

            var createdProteins = _proteinRepository.CreateMissing(allProteins, existingProteins, transcriptsCache, genesCache).ToArray();

            var proteinsCache = Enumerable.Concat(createdProteins, existingProteins).ToArray();


            var expressions = _proteinExpressionRepository.CreateAll(sampleId, chunk, proteinsCache).ToArray();


            audit.GenesCreated += createdGenes.Length;

            audit.TranscriptsCreated += createdTranscripts.Length;

            audit.ProteinsCreated += createdProteins.Length;

            audit.ExpressionsCreated += expressions.Length;

            audit.Proteins.AddRange(expressions.Select(entity => entity.EntityId));
        }
    }
}
