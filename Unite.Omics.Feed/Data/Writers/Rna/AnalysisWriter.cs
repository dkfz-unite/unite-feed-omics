using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Data.Models.Rna;
using Unite.Omics.Feed.Data.Repositories;
using Unite.Omics.Feed.Data.Repositories.Rna;

namespace Unite.Omics.Feed.Data.Writers.Rna;

public class AnalysisWriter : Writers.AnalysisWriter<Models.AnalysisModel<GeneExpressionModel>, AnalysisWriteAudit>
{
    private const int _batchSize = 1000;


    private readonly IGenomeOptions _genomeOptions;


    private GeneRepository _geneRepository;
    private ExpressionRepository _geneExpressionRepository;


    public AnalysisWriter(IDbContextFactory<DomainDbContext> dbContextFactory, IGenomeOptions genomeOptions) : base(dbContextFactory)
    {
        _genomeOptions = genomeOptions;
    }


    protected override void InitializeEntries(DomainDbContext dbContext)
    {
        _geneRepository = new GeneRepository(dbContext, _genomeOptions);
        _geneExpressionRepository = new ExpressionRepository(dbContext, _genomeOptions);
    }

    protected override void WriteEntries(int sampleId, Models.AnalysisModel<GeneExpressionModel> model, ref AnalysisWriteAudit audit)
    {
        var models = model.Entries;
        if (models.IsEmpty())
            return;

        var queue = new Queue<GeneExpressionModel>(models);

        var removedGenes = _geneExpressionRepository.RemoveAll(sampleId);

        while (queue.Any())
        {
            var chunk = queue.Dequeue(_batchSize).ToArray();

            var existingGenes = _geneRepository.Find(chunk.Select(model => model.Gene)).ToArray();
            var createdGenes = _geneRepository.CreateMissing(chunk.Select(model => model.Gene), existingGenes).ToArray();
            var genesCache = Enumerable.Concat(createdGenes, existingGenes).ToArray();

            var geneExpressions = _geneExpressionRepository.CreateAll(sampleId, chunk, genesCache).ToArray();

            audit.GenesCreated += createdGenes.Length;
            audit.ExpressionsCreated += geneExpressions.Length;
            audit.Genes.AddRange(removedGenes);
            audit.Genes.AddRange(geneExpressions.Select(entity => entity.EntityId));
        }
    }
}
