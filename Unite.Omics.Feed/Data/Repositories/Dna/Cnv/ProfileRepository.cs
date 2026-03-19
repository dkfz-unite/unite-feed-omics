using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Omics.Feed.Data.Models.Dna.Cnv;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Cnv;

public class ProfileRepository(DomainDbContext dbContext)
{
    public void CreateOrUpdate(int sampleId, IEnumerable<ProfileModel> models,
        ref ISet<int> createdEntities, ref ISet<int> updatedEntities)
    {
        var newEntities = new List<Profile>();
        
        foreach (var model in models)
        {
            bool isNewEntity = false;
            var entity = CreateOrUpdate(sampleId, model, ref isNewEntity);
            if (isNewEntity)
            {
                newEntities.Add(entity);
            }
            else
            {
                updatedEntities.Add(entity.Id);
            }
        }
        
        dbContext.SaveChanges();

        //Track new entities after they are saved and have unique id set
        foreach (var entity in newEntities)
        {
            createdEntities.Add(entity.Id);
        }
    }

    public Profile CreateOrUpdate(int sampleId, ProfileModel model, ref bool isNewEntity)
    {
        var entity = Find(sampleId, model);
        if (entity == null)
        {
            entity = new Profile
            {
                SampleId = sampleId,
                ChromosomeId = model.Chromosome,
                ChromosomeArmId = model.ChromosomeArm,
            };
            
            isNewEntity = true;
            
            dbContext.Add(entity);
        }
            
        entity.Gain = model.Gain;
        entity.Loss = model.Loss;
        entity.Neutral = model.Neutral;
        
        return entity;
    }
    
    public Profile Find(int sampleId, ProfileModel model)
    {
        return dbContext.Set<Profile>()
            .FirstOrDefault(e => 
                e.SampleId == sampleId 
                && e.ChromosomeId == model.Chromosome 
                && e.ChromosomeArmId == model.ChromosomeArm);
    }
}