using Unite.Data.Context;
using Unite.Data.Entities.Omics;
using Unite.Data.Entities.Omics.Analysis.Prot;
using Unite.Omics.Feed.Data.Models.Prot;

namespace Unite.Omics.Feed.Data.Repositories.Prot;

public class ExpressionRepository
{
    private readonly DomainDbContext _dbContext;
    private readonly ProteinRepository _proteinRepository;


    public ExpressionRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
        _proteinRepository = new ProteinRepository(dbContext);
    }


    public ProteinExpression FindOrCreate(int sampleId, ProteinExpressionModel model)
    {
        return Find(sampleId, model) ?? Create(sampleId, model);
    }

    public ProteinExpression Find(int sampleId, ProteinExpressionModel model, IEnumerable<Protein> proteinsCache = null)
    {
        var protein = _proteinRepository.Find(model.Protein, proteinsCache);

        if (protein != null)
        {
            return Find(sampleId, protein.Id);
        }

        return null;
    }

    public ProteinExpression Create(int sampleId, ProteinExpressionModel model, IEnumerable<Protein> proteinsCache = null)
    {
        var protein = _proteinRepository.FindOrCreate(model.Protein, proteinsCache);

        return Create(sampleId, protein.Id, model);
    }

    public IEnumerable<ProteinExpression> CreateAll(int sampleId, IEnumerable<ProteinExpressionModel> models, IEnumerable<Protein> proteinsCache = null)
    {
        var entitiesToAdd = new List<ProteinExpression>();

        foreach (var model in models)
        {
            var protein = _proteinRepository.FindOrCreate(model.Protein, proteinsCache);

            var entity = Convert(sampleId, protein.Id, model);

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
        var entitiesToRemove = _dbContext.Set<ProteinExpression>().Where(entity => entity.SampleId == sampleId).ToArray();

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private ProteinExpression Find(int sampleId, int proteinId)
    {
        var entity = _dbContext.Set<ProteinExpression>()
            .FirstOrDefault(
                entity => entity.SampleId == sampleId &&
                entity.EntityId == proteinId
            );

        return entity;
    }

    private ProteinExpression Create(int sampleId, int proteinId, ProteinExpressionModel model)
    {
        var entity = Convert(sampleId, proteinId, model);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    private ProteinExpression Convert(int sampleId, int proteinId, ProteinExpressionModel model)
    {
        var entity = new ProteinExpression
        {
            SampleId = sampleId,
            EntityId = proteinId
        };

        Map(model, ref entity);

        return entity;
    }

    private static void Map(ProteinExpressionModel model, ref ProteinExpression entity)
    {
        entity.Intensity = model.Intensity;
        entity.MedianCenteredLog2 = model.MedianCenteredLog2;
    }
}
