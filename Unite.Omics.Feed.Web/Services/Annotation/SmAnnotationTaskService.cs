using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;

namespace Unite.Omics.Feed.Web.Services.Annotation;

public class SmAnnotationTaskService : VariantAnnotationTaskService<Variant>
{
    public SmAnnotationTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
