using Unite.Data.Entities.Genome.Variants.CNV;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Web.Services;

public class CopyNumberVariantIndexingTaskService : VariantIndexingTaskService<Variant, VariantOccurrence, AffectedTranscript>
{
    public CopyNumberVariantIndexingTaskService(DomainDbContext dbContext) : base(dbContext)
    {
    }
}
