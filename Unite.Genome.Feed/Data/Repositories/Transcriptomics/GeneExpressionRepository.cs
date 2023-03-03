using Unite.Data.Entities.Genome;
using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Transcriptomics;

namespace Unite.Genome.Feed.Data.Repositories.Transcriptomics;

public class GeneExpressionRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly GeneRepository _geneRepository;


    public GeneExpressionRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _geneRepository = new GeneRepository(dbContext);
    }


    public GeneExpression FindOrCreate(int analysedSampleId, GeneExpressionModel model)
    {
        return Find(analysedSampleId, model) ?? Create(analysedSampleId, model);
    }

    public GeneExpression Find(int analysedSampleId, GeneExpressionModel model, IEnumerable<Gene> cache = null)
    {
        var gene = _geneRepository.Find(model.Gene, cache);

        if (gene != null)
        {
            return Find(analysedSampleId, gene.Id);
        }

        return null;
    }

    public GeneExpression Create(int analysedSampleId, GeneExpressionModel model, IEnumerable<Gene> cache = null)
    {
        var gene = _geneRepository.FindOrCreate(model.Gene, cache);

        return Create(analysedSampleId, gene.Id, model);
    }

    public IEnumerable<GeneExpression> CreateAll(int analysedSampleId, IEnumerable<GeneExpressionModel> models, IEnumerable<Gene> cache = null)
    {
        var entitiesToAdd = new List<GeneExpression>();

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
        var entitiesToRemove = _dbContext.Set<GeneExpression>().Where(entity => entity.AnalysedSampleId == analysedSampleId).ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private GeneExpression Find(int analysedSampleId, int geneid)
    {
        var entity = _dbContext.Set<GeneExpression>()
            .FirstOrDefault(entity =>
                entity.AnalysedSampleId == analysedSampleId &&
                entity.GeneId == geneid
            );

        return entity;
    }

    private GeneExpression Create(int analysedSampleId, int geneId, GeneExpressionModel model)
    {
        var entity = Convert(analysedSampleId, geneId, model);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private GeneExpression Convert(int analysedSampleId, int geneId, GeneExpressionModel model)
    {
        var entity = new GeneExpression();

        entity.AnalysedSampleId = analysedSampleId;

        entity.GeneId = geneId;

        Map(model, ref entity);

        return entity;
    }

    private void Map(GeneExpressionModel model, ref GeneExpression entity)
    {
        entity.Reads = model.Reads;
        entity.TPM = model.TPM;
        entity.FPKM = model.FPKM;
    }
}
