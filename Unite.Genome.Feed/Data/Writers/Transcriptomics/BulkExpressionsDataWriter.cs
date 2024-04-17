using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Context.Services;
using Unite.Essentials.Extensions;
using Unite.Genome.Feed.Data.Models;
using Unite.Genome.Feed.Data.Models.Transcriptomics;
using Unite.Genome.Feed.Data.Repositories;
using Unite.Genome.Feed.Data.Repositories.Transcriptomics;

namespace Unite.Genome.Feed.Data.Writers.Transcriptomics;

public class BulkExpressionsDataWriter : DataWriter<AnalysedSampleModel, BulkExpressionsDataUploadAudit>
{
    private const int _batchSize = 1000;

    private AnalysisRepository _analysisRepository;
    private AnalysedSampleRepository _analysedSampleRepository;
    private GeneRepository _geneRepository;
    private BulkExpressionRepository _bulkExpressionRepository;


    public BulkExpressionsDataWriter(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
        var dbContext = dbContextFactory.CreateDbContext();
        
        Initialize(dbContext);
    }

    
    protected override void Initialize(DomainDbContext dbContext)
    {
        _analysisRepository = new AnalysisRepository(dbContext);
        _analysedSampleRepository = new AnalysedSampleRepository(dbContext);
        _geneRepository = new GeneRepository(dbContext);
        _bulkExpressionRepository = new BulkExpressionRepository(dbContext);
    }

    protected override void ProcessModel(AnalysedSampleModel model, ref BulkExpressionsDataUploadAudit audit)
    {
        var analysis = _analysisRepository.FindOrCreate(model);

        var analysedSample = _analysedSampleRepository.FindOrCreate(analysis.Id, model);

        if (model.BulkExpressions != null)
        {
            WriteExpressions(analysedSample.Id, model.BulkExpressions, ref audit);
        }
    }


    private void WriteExpressions(int analysedSampleId, IEnumerable<BulkExpressionModel> models, ref BulkExpressionsDataUploadAudit audit)
    {
        var queue = new Queue<BulkExpressionModel>(models);

        _bulkExpressionRepository.RemoveAll(analysedSampleId);

        while (queue.Any())
        {
            var chunk = queue.Dequeue(_batchSize).ToArray();

            var existingGenes = _geneRepository.Find(chunk.Select(model => model.Gene)).ToArray();

            var createdGenes = _geneRepository.CreateMissing(chunk.Select(model => model.Gene), existingGenes).ToArray();

            var genesCache = Enumerable.Concat(createdGenes, existingGenes).ToArray();


            var geneExpressions = _bulkExpressionRepository.CreateAll(analysedSampleId, chunk, genesCache).ToArray();


            audit.GenesCreated += createdGenes.Length;

            audit.ExpressionsAssociated += geneExpressions.Length;

            audit.Genes.AddRange(geneExpressions.Select(entity => entity.EntityId));
        }
    }
}
