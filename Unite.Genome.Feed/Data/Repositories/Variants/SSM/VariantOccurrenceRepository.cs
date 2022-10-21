using Unite.Data.Entities.Genome.Variants.SSM;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Variants.SSM;

namespace Unite.Genome.Feed.Data.Repositories.Variants.SSM;

internal class VariantOccurrenceRepository : VariantOccurrenceRepository<VariantOccurrence, Variant, VariantModel>
{
    public VariantOccurrenceRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
