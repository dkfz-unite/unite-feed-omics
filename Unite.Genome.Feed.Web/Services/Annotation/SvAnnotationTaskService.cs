using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Variants.SV;

namespace Unite.Genome.Feed.Web.Services.Annotation;

public class SvAnnotationTaskService : VariantAnnotationTaskService<Variant>
{
    public SvAnnotationTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
