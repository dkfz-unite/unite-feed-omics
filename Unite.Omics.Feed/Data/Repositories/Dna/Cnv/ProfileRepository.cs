using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Omics.Feed.Data.Models.Dna.Cnv;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Cnv;

public class ProfileRepository(DomainDbContext dbContext)
{
    public IEnumerable<Profile> CreateOrUpdate(int sampleId, IEnumerable<ProfileModel> models)
    {
        var entitiesToAdd = new List<Profile>();

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

    public Profile CreateOrUpdate(int sampleId, ProfileModel model)
    {
        var entity = Find(sampleId, model);
        if (entity == null)
        {
            entity = new Profile
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
    
    public Profile Find(int sampleId, ProfileModel model)
    {
        return dbContext.Set<Profile>()
            .FirstOrDefault(e => 
                e.SampleId == sampleId 
                && e.Chromosome == model.Chromosome 
                && e.ChromosomeArm == model.ChromosomeArm);
    }
}