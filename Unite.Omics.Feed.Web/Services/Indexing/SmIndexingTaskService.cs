using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;

namespace Unite.Omics.Feed.Web.Services.Indexing;

public class SmIndexingTaskService : VariantIndexingTaskService<Variant>
{
    public SmIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
