using Unite.Data.Entities.Genome.Variants.SV;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Web.Services.Indexing;

public class StructuralVariantIndexingTaskService : VariantIndexingTaskService<Variant, VariantOccurrence, AffectedTranscript>
{
    public StructuralVariantIndexingTaskService(DomainDbContext dbContext) : base(dbContext)
    {
    }
}
