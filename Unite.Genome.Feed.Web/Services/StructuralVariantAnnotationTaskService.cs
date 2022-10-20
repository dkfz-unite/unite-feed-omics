using Unite.Data.Entities.Genome.Variants.SV;
using Unite.Data.Services;

namespace Unite.Genome.Feed.Web.Services;

public class StructuralVariantAnnotationTaskService : VariantAnnotationTaskService<Variant>
{
    public StructuralVariantAnnotationTaskService(DomainDbContext dbContext) : base(dbContext)
    {
    }
}
