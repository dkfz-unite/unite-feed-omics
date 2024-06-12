using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Cnv;

namespace Unite.Genome.Feed.Web.Services.Annotation;

public class CnvAnnotationTaskService : VariantAnnotationTaskService<Variant>
{
    public CnvAnnotationTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
