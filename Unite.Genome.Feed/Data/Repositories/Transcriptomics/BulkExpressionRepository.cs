using Unite.Data.Context;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Genome.Feed.Data.Models.Transcriptomics;

namespace Unite.Genome.Feed.Data.Repositories.Transcriptomics;

public class BulkExpressionRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly GeneRepository _geneRepository;


    public BulkExpressionRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _geneRepository = new GeneRepository(dbContext);
    }


    public BulkExpression FindOrCreate(int analysedSampleId, BulkExpressionModel model)
    {
        return Find(analysedSampleId, model) ?? Create(analysedSampleId, model);
    }

    public BulkExpression Find(int analysedSampleId, BulkExpressionModel model, IEnumerable<Gene> cache = null)
    {
        var gene = _geneRepository.Find(model.Gene, cache);

        if (gene != null)
        {
            return Find(analysedSampleId, gene.Id);
        }

        return null;
    }

    public BulkExpression Create(int analysedSampleId, BulkExpressionModel model, IEnumerable<Gene> cache = null)
    {
        var gene = _geneRepository.FindOrCreate(model.Gene, cache);

        return Create(analysedSampleId, gene.Id, model);
    }

    public IEnumerable<BulkExpression> CreateAll(int analysedSampleId, IEnumerable<BulkExpressionModel> models, IEnumerable<Gene> cache = null)
    {
        var entitiesToAdd = new List<BulkExpression>();

        foreach (var model in models)
        {
            var gene = _geneRepository.FindOrCreate(model.Gene, cache);

            var entity = Convert(analysedSampleId, gene.Id, model);

            entitiesToAdd.Add(entity);
        }

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd;
    }

    public void RemoveAll(int analysedSampleId)
    {
        var entitiesToRemove = _dbContext.Set<BulkExpression>().Where(entity => entity.AnalysedSampleId == analysedSampleId).ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private BulkExpression Find(int analysedSampleId, int geneId)
    {
        var entity = _dbContext.Set<BulkExpression>()
            .FirstOrDefault(entity =>
                entity.AnalysedSampleId == analysedSampleId &&
                entity.EntityId == geneId
            );

        return entity;
    }

    private BulkExpression Create(int analysedSampleId, int geneId, BulkExpressionModel model)
    {
        var entity = Convert(analysedSampleId, geneId, model);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private BulkExpression Convert(int analysedSampleId, int geneId, BulkExpressionModel model)
    {
        var entity = new BulkExpression
        {
            AnalysedSampleId = analysedSampleId,
            EntityId = geneId
        };

        Map(model, ref entity);

        return entity;
    }

    private static void Map(BulkExpressionModel model, ref BulkExpression entity)
    {
        entity.Reads = model.Reads;
        entity.TPM = model.TPM;
        entity.FPKM = model.FPKM;
    }
}
