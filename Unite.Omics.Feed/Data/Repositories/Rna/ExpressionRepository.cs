using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Data.Entities.Omics.Analysis.Rna;
using Unite.Omics.Feed.Data.Configuration;
using Unite.Omics.Feed.Data.Models.Rna;

namespace Unite.Omics.Feed.Data.Repositories.Rna;

public class ExpressionRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly GeneRepository _geneRepository;


    public ExpressionRepository(DomainDbContext dbContext, IGenomeOptions genomeOptions)
    {
        _dbContext = dbContext;
        _geneRepository = new GeneRepository(dbContext, genomeOptions);
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

    /// <summary>
    /// Removes all gene expression entries associated with the specified sample.
    /// </summary>
    /// <param name="sampleId">Sample identifier.</param>
    /// <returns>Gene identifiers of the removed entries.</returns>
    public int[] RemoveAll(int sampleId)
    {
        var query = _dbContext.Set<GeneExpression>()
            .Where(entity => entity.SampleId == sampleId);

        var ids = query.Select(entity => entity.EntityId).ToArray();

        query.ExecuteDelete();

        return ids;
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
        entity.Raw = model.Raw;
        entity.TPM = model.TPM;
        entity.FPKM = model.FPKM;
        entity.Normalized = model.Normalized;
    }
}
