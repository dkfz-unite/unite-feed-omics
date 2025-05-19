using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Omics.Feed.Data.Models.Dna.Cnv;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Cnv;

public class VariantEntryRepository : VariantEntryRepository<VariantEntry, Variant, VariantModel>
{
    public VariantEntryRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
