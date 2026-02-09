using Unite.Data.Context;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Repositories.CnvProfile;

public class CnvProfileRepository(DomainDbContext dbContext)
{
    private readonly DomainDbContext _dbContext = dbContext;

    public IEnumerable<StubEntities.CnvProfile> CreateAll(int sampleId, IEnumerable<CnvProfileModel> models)
    {
        var entitiesToAdd = new List<StubEntities.CnvProfile>();

        foreach (var model in models)
        {
            //TODO: create entity properly
            var entity = new StubEntities.CnvProfile();
            
            entitiesToAdd.Add(entity);
        }
        
        if (entitiesToAdd.Any())
        {
            _dbContext.AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }
        
        return entitiesToAdd;
    }
}