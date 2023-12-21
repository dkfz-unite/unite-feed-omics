using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Genome.Feed.Web.Services.Indexing;

public class SvIndexingTaskService : VariantIndexingTaskService<Variant>
{
    public SvIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
