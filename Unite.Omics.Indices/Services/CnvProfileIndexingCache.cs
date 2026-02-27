using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Specimens;

namespace Unite.Omics.Indices.Services;

public class CnvProfileIndexingCache(IDbContextFactory<DomainDbContext> dbContextFactory) : IndexingCache(dbContextFactory)
{
    public IEnumerable<Specimen> Specimens { get; private set; }
    public IEnumerable<Data.Entities.Omics.Analysis.Sample> Samples { get; private set; }

    protected override void Load(int[] ids)
    {
        throw new NotImplementedException();
    }
}