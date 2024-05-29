using Unite.Data.Context;
using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Analysis.Rna;
using Unite.Genome.Feed.Data.Models.Rna;

namespace Unite.Genome.Feed.Data.Repositories.Rna;

public class ExpressionRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly GeneRepository _geneRepository;


    public ExpressionRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _geneRepository = new GeneRepository(dbContext);
    }


    public GeneExpression FindOrCreate(int sampleId, GeneExpressionModel model)
    {
        return Find(sampleId, model) ?? Create(sampleId, model);
    }

    public GeneExpression Find(int sampleId, GeneExpressionModel model, IEnumerable<Gene> cache = null)
    {
        var gene = _geneRepository.Find(model.Gene, cache);

        if (gene != null)
        {
            return Find(sampleId, gene.Id);
        }

        return null;
    }

    public GeneExpression Create(int sampleId, GeneExpressionModel model, IEnumerable<Gene> cache = null)
    {
        var gene = _geneRepository.FindOrCreate(model.Gene, cache);

        return Create(sampleId, gene.Id, model);
    }

    public IEnumerable<GeneExpression> CreateAll(int sampleId, IEnumerable<GeneExpressionModel> models, IEnumerable<Gene> cache = null)
    {
        var entitiesToAdd = new List<GeneExpression>();

        foreach (var model in models)
        {
            var gene = _geneRepository.FindOrCreate(model.Gene, cache);

            var entity = Convert(sampleId, gene.Id, model);

            entitiesToAdd.Add(entity);
        }

        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        return entitiesToAdd;
    }

    public void RemoveAll(int sampleId)
    {
        var entitiesToRemove = _dbContext.Set<GeneExpression>().Where(entity => entity.SampleId == sampleId).ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private GeneExpression Find(int sampleId, int geneId)
    {
        var entity = _dbContext.Set<GeneExpression>()
            .FirstOrDefault(entity =>
                entity.SampleId == sampleId &&
                entity.EntityId == geneId
            );

        return entity;
    }

    private GeneExpression Create(int sampleId, int geneId, GeneExpressionModel model)
    {
        var entity = Convert(sampleId, geneId, model);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private GeneExpression Convert(int sampleId, int geneId, GeneExpressionModel model)
    {
        var entity = new GeneExpression
        {
            SampleId = sampleId,
            EntityId = geneId
        };

        Map(model, ref entity);

        return entity;
    }

    private static void Map(GeneExpressionModel model, ref GeneExpression entity)
    {
        entity.Reads = model.Reads;
        entity.TPM = model.TPM;
        entity.FPKM = model.FPKM;
    }
}
