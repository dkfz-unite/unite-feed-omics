using Unite.Data.Entities.Genome.Variants.SV;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Variants.SV;

namespace Unite.Genome.Feed.Data.Repositories.Variants.SV;

internal class VariantOccurrenceRepository : VariantOccurrenceRepository<VariantOccurrence, Variant, VariantModel>
{
    public VariantOccurrenceRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
