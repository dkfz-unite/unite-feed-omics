using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Ssm;

namespace Unite.Genome.Feed.Web.Services.Indexing;

public class SsmIndexingTaskService : VariantIndexingTaskService<Variant>
{
    public SsmIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
