using Unite.Data.Entities.Genome.Variants.CNV;
using Unite.Data.Services;
using Unite.Genome.Feed.Data.Models.Variants.CNV;

namespace Unite.Genome.Feed.Data.Repositories.Variants.CNV;

public class VariantOccurrenceRepository : VariantOccurrenceRepository<VariantOccurrence, Variant, VariantModel>
{
    public VariantOccurrenceRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
