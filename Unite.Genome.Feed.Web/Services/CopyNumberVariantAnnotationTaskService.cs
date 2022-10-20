using Unite.Data.Entities.Genome.Variants.CNV;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Web.Services;

public class CopyNumberVariantAnnotationTaskService : VariantAnnotationTaskService<Variant>
{
    public CopyNumberVariantAnnotationTaskService(DomainDbContext dbContext) : base(dbContext)
    {
    }
}
