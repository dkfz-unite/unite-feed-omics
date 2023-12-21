using Unite.Data.Context;
using Unite.Data.Entities.Genome.Variants.SV;
using Unite.Genome.Feed.Data.Models.Variants.SV;

namespace Unite.Genome.Feed.Data.Repositories.Variants.SV;

public class VariantEntryRepository : VariantEntryRepository<VariantEntry, Variant, VariantModel>
{
    public VariantEntryRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
