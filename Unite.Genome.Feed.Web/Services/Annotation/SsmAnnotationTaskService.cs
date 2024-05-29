using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Ssm;

namespace Unite.Genome.Feed.Web.Services.Annotation;

public class SsmAnnotationTaskService : VariantAnnotationTaskService<Variant>
{
    public SsmAnnotationTaskService(IDbContextFactory<DomainDbContext> dbContextFactory) : base(dbContextFactory)
    {
    }
}
