using Unite.Data.Entities.Genome.Variants.SSM;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Web.Services.Indexing;

public class MutationIndexingTaskService : VariantIndexingTaskService<Variant, VariantOccurrence, AffectedTranscript>
{
    public MutationIndexingTaskService(DomainDbContext dbContext) : base(dbContext)
    {
    }
}
