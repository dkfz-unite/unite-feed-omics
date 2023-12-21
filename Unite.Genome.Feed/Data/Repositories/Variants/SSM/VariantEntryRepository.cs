using Unite.Data.Context;
using Unite.Data.Entities.Genome.Variants.SSM;
using Unite.Genome.Feed.Data.Models.Variants.SSM;

namespace Unite.Genome.Feed.Data.Repositories.Variants.SSM;

public class VariantEntryRepository : VariantEntryRepository<VariantEntry, Variant, VariantModel>
{
    public VariantEntryRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
