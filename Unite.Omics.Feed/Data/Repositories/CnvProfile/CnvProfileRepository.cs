using Unite.Omics.Feed.Data.Models;
using Unite.Omics.Feed.Data.Models.Dna;

namespace Unite.Omics.Feed.Data.Repositories.Dna;

public class CnvProfileRepository
{
    public IEnumerable<StubEntities.CnvProfile> CreateAll(int sampleId, IEnumerable<CnvProfileModel> models)
    {
        var entitiesToAdd = new List<StubEntities.CnvProfile>();

        foreach (var model in models)
        {
            var entity = new StubEntities.CnvProfile();
            
            entitiesToAdd.Add(entity);
        }
        
        return entitiesToAdd;
    }
}