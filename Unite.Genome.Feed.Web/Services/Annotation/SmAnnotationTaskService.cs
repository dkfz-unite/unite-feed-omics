using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Sm;

namespace Unite.Genome.Feed.Web.Services.Annotation;

public class SmAnnotationTaskService : VariantAnnotationTaskService<Variant>
{
    public SmAnnotationTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
