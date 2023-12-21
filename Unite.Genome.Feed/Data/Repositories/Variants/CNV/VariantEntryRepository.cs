using Unite.Data.Context;
using Unite.Data.Entities.Genome.Variants.CNV;
using Unite.Genome.Feed.Data.Models.Variants.CNV;

namespace Unite.Genome.Feed.Data.Repositories.Variants.CNV;

public class VariantEntryRepository : VariantEntryRepository<VariantEntry, Variant, VariantModel>
{
    public VariantEntryRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
