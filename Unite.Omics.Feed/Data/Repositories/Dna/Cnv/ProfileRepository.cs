using Unite.Data.Context;
using Unite.Omics.Feed.Data.Models.Dna.Cnv;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Cnv;

public class ProfileRepository(DomainDbContext dbContext)
{
    public IEnumerable<StubEntities.CnvProfile> CreateOrUpdate(int sampleId, IEnumerable<ProfileModel> models)
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

    public StubEntities.CnvProfile CreateOrUpdate(int sampleId, ProfileModel model)
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
    
    public StubEntities.CnvProfile Find(int sampleId, ProfileModel model)
    {
        return dbContext.Set<StubEntities.CnvProfile>()
            .FirstOrDefault(e => 
                e.SampleId == sampleId 
                && e.Chromosome == model.Chromosome 
                && e.ChromosomeArm == model.ChromosomeArm);
    }
}