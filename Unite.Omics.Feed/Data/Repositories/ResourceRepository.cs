using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Repositories;

public class ResourceRepository
{
    private readonly DomainDbContext _dbContext;


    public ResourceRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public SampleResource FindOrCreate(int sampleId, ResourceModel model)
    {
        return Find(sampleId, model) ?? Create(sampleId, model);
    }

    public SampleResource Find(int sampleId, ResourceModel model)
    {
        return _dbContext.Set<SampleResource>()
            .FirstOrDefault(entity => 
                entity.SampleId == sampleId &&
                entity.Name == model.Name && 
                entity.Type == model.Type &&
                entity.Format == model.Format
            );
    }

    public SampleResource Create(int sampleId, ResourceModel model)
    {
        var entity = Convert(sampleId, model);

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public void Update(SampleResource entity, ResourceModel model)
    {
        entity.Format = model.Format;
        entity.Url = model.Url;

        _dbContext.Update(entity);
        _dbContext.SaveChanges();
    }

    private static SampleResource Convert(int sampleId, ResourceModel model)
    {
        return new SampleResource
        {
            SampleId = sampleId,
            Name = model.Name,
            Type = model.Type,
            Format = model.Format,
            Archive = model.Archive,
            Url = model.Url
        };
    }
}
