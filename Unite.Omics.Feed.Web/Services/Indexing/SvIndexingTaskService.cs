using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;

namespace Unite.Omics.Feed.Web.Services.Indexing;

public class SvIndexingTaskService : VariantIndexingTaskService<Variant>
{
    public SvIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
