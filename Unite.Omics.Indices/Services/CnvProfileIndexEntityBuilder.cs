using Unite.Indices.Entities.CnvProfiles;

namespace Unite.Omics.Indices.Services;

public class CnvProfileIndexEntityBuilder: IndexEntityBuilder<CnvProfileIndex, CnvProfileIndexingCache>
{
    public override CnvProfileIndex Create(int key, CnvProfileIndexingCache cache)
    {
        throw new NotImplementedException();
    }
}