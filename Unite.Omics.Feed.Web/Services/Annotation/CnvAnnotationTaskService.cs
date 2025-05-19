using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;

namespace Unite.Omics.Feed.Web.Services.Annotation;

public class CnvAnnotationTaskService : VariantAnnotationTaskService<Variant>
{
    public CnvAnnotationTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
