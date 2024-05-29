using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Sv;
using Unite.Genome.Feed.Data.Models.Dna.Sv;

namespace Unite.Genome.Feed.Data.Repositories.Dna.Sv;

public class VariantEntryRepository : VariantEntryRepository<VariantEntry, Variant, VariantModel>
{
    public VariantEntryRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
