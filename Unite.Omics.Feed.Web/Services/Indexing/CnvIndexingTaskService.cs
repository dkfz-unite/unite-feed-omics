using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;

namespace Unite.Omics.Feed.Web.Services.Indexing;

public class CnvIndexingTaskService : VariantIndexingTaskService<Variant>
{
    public CnvIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
