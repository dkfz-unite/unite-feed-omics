using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Sm;
using Unite.Genome.Feed.Data.Models.Dna.Sm;

namespace Unite.Genome.Feed.Data.Repositories.Dna.Sm;

public class VariantEntryRepository : VariantEntryRepository<VariantEntry, Variant, VariantModel>
{
    public VariantEntryRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
