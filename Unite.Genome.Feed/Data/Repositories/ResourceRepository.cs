using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis;
using Unite.Genome.Feed.Data.Models;

namespace Unite.Genome.Feed.Data.Repositories;

public class ResourceRepository
{
    private readonly DomainDbContext _dbContext;


    public ResourceRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public AnalysedSampleResource FindOrCreate(int analysedSampleId, ResourceModel model)
    {
        return Find(analysedSampleId, model) ?? Create(analysedSampleId, model);
    }

    public AnalysedSampleResource Find(int analysedSampleId, ResourceModel model)
    {
        var entity =  _dbContext.Set<AnalysedSampleResource>()
            .FirstOrDefault(entity => 
                entity.AnalysedSampleId == analysedSampleId && 
                entity.Type == model.Type
            );

        return entity;
    }

    public AnalysedSampleResource Create(int analysedSampleId, ResourceModel model)
    {
        var entity = Convert(analysedSampleId, model);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public IEnumerable<AnalysedSampleResource> CreateAll(int analysedSampleId, IEnumerable<ResourceModel> models)
    {
        var entitiesToAdd = new List<AnalysedSampleResource>();

        foreach (var model in models)
        {
            var entity = Convert(analysedSampleId, model);

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
        var entitiesToRemove = _dbContext.Set<AnalysedSampleResource>()
            .Where(entity => entity.AnalysedSampleId == analysedSampleId);

        if (entitiesToRemove.Any())
        {
            _dbContext.RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }


    private static AnalysedSampleResource Convert(int analysedSampleId, ResourceModel model)
    {
        return new AnalysedSampleResource
        {
            AnalysedSampleId = analysedSampleId,
            Type = model.Type,
            Path = model.Path,
            Url = model.Url
        };
    }
}
