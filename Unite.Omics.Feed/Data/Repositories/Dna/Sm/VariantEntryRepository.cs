using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Omics.Feed.Data.Models.Dna.Sm;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Sm;

public class VariantEntryRepository : VariantEntryRepository<VariantEntry, Variant, VariantModel>
{
    public VariantEntryRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
