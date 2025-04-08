using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Sm;

namespace Unite.Genome.Feed.Web.Services.Indexing;

public class SmIndexingTaskService : VariantIndexingTaskService<Variant>
{
    public SmIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
