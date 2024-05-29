using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Sv;

namespace Unite.Genome.Feed.Web.Services.Indexing;

public class SvIndexingTaskService : VariantIndexingTaskService<Variant>
{
    public SvIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
