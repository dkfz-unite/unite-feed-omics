using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Models.Rna;
using Unite.Genome.Feed.Data.Repositories;
using Unite.Genome.Feed.Data.Repositories.Rna;

namespace Unite.Genome.Feed.Data.Writers.Rna;

public class AnalysisWriter : DataWriter<SampleModel, AnalysisWriteAudit>
{
    private const int _batchSize = 1000;

    private GeneRepository _geneRepository;
    private GeneExpressionRepository _geneExpressionRepository;

    public AnalysisWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    protected override void Initialize(DomainDbContext dbContext)
    {
        _sampleRepository = new SampleRepository(dbContext);
        _geneRepository = new GeneRepository(dbContext);
        _geneExpressionRepository = new GeneExpressionRepository(dbContext);
        _resourceRepository = new ResourceRepository(dbContext);
    }

    protected override void ProcessModel(SampleModel model, ref AnalysisWriteAudit audit)
    {
        var sampleId = WriteSample(model, ref audit);

        if (model.Exps.IsNotEmpty())
            WriteExpressions(sampleId, model.Exps, ref audit);

        if (model.Resources.IsNotEmpty())
            WriteResources(sampleId, model.Resources, ref audit);
    }


    private void WriteExpressions(int sampleId, IEnumerable<GeneExpressionModel> models, ref AnalysisWriteAudit audit)
    {
        var queue = new Queue<GeneExpressionModel>(models);

        _geneExpressionRepository.RemoveAll(sampleId);

        while (queue.Any())
        {
            var chunk = queue.Dequeue(_batchSize).ToArray();

            var existingGenes = _geneRepository.Find(chunk.Select(model => model.Gene)).ToArray();

            var createdGenes = _geneRepository.CreateMissing(chunk.Select(model => model.Gene), existingGenes).ToArray();

            var genesCache = Enumerable.Concat(createdGenes, existingGenes).ToArray();


            var geneExpressions = _geneExpressionRepository.CreateAll(sampleId, chunk, genesCache).ToArray();


            audit.GenesCreated += createdGenes.Length;

            audit.ExpressionsCreated += geneExpressions.Length;

            audit.Genes.AddRange(geneExpressions.Select(entity => entity.EntityId));
        }
    }
}
