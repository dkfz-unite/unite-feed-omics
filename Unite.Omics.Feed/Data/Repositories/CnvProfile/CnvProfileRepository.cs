using Unite.Data.Context;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Repositories.CnvProfile;

public class CnvProfileRepository(DomainDbContext dbContext)
{
    public IEnumerable<StubEntities.CnvProfile> CreateAll(int sampleId, IEnumerable<CnvProfileModel> models)
    {
        var entitiesToAdd = new List<StubEntities.CnvProfile>();

        foreach (var model in models)
        {
            var entity = CreateOrUpdate(sampleId, model);
            entitiesToAdd.Add(entity);
        }
        
        if (entitiesToAdd.Any())
        {
            dbContext.AddRange(entitiesToAdd);
            dbContext.SaveChanges();
        }
        
        return entitiesToAdd;
    }

    public StubEntities.CnvProfile CreateOrUpdate(int sampleId, CnvProfileModel model)
    {
        var entity = Find(sampleId, model);
        if (entity == null)
        {
            entity = new StubEntities.CnvProfile
            {
                SampleId = sampleId,
                Chromosome = model.Chromosome,
                ChromosomeArm = model.ChromosomeArm,
            };
        }
            
        entity.Gain = model.Gain;
        entity.Loss = model.Loss;
        entity.Neutral = model.Neutral;
        
        return entity;
    }
    
    public StubEntities.CnvProfile Find(int sampleId, CnvProfileModel model)
    {
        return dbContext.Set<StubEntities.CnvProfile>()
            .FirstOrDefault(e => 
                e.SampleId == sampleId 
                && e.Chromosome == model.Chromosome 
                && e.ChromosomeArm == model.ChromosomeArm);
    }
}