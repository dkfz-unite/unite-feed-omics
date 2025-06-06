using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Omics.Feed.Data.Models.Dna.Sv;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Sv;

public class VariantEntryRepository : VariantEntryRepository<VariantEntry, Variant, VariantModel>
{
    public VariantEntryRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
