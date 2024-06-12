using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Cnv;

namespace Unite.Genome.Feed.Web.Services.Indexing;

public class CnvIndexingTaskService : VariantIndexingTaskService<Variant>
{
    public CnvIndexingTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
