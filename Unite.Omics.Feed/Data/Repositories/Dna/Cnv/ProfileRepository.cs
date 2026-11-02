using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Omics.Feed.Data.Models.Dna.Cnv;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Cnv;

public class ProfileRepository
{
    private readonly DomainDbContext _dbContext;


    public ProfileRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Profile[] CreateAll(int sampleId, IEnumerable<ProfileModel> models)
    {
        var entities = models.Select(model => new Profile
        {
            SampleId = sampleId,
            ChromosomeId = model.Chromosome,
            ChromosomeArmId = model.ChromosomeArm,
            Gain = model.Gain,
            Loss = model.Loss,
            Neutral = model.Neutral
        }).ToArray();

        _dbContext.AddRange(entities);
        _dbContext.SaveChanges();

        return entities;
    }

    /// <summary>
    /// Removes all entities associated with the specified sample.
    /// </summary>
    /// <param name="sampleId">Sample identifier.</param>
    /// <returns>Identifiers of the removed entities.</returns>
    public int[] RemoveAll(int sampleId)
    {
        var query = _dbContext.Set<Profile>()
            .Where(entity => entity.SampleId == sampleId);
            
        var ids = query.Select(entity => entity.Id).ToArray();

        query.ExecuteDelete();

        return ids;
    }
}
