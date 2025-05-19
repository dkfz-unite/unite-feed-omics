using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;

namespace Unite.Omics.Feed.Web.Services.Annotation;

public class SvAnnotationTaskService : VariantAnnotationTaskService<Variant>
{
    public SvAnnotationTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
