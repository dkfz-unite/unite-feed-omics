using Unite.Data.Context;
using Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using Unite.Genome.Feed.Data.Models.Dna.Ssm;

namespace Unite.Genome.Feed.Data.Repositories.Dna.Ssm;

public class VariantEntryRepository : VariantEntryRepository<VariantEntry, Variant, VariantModel>
{
    public VariantEntryRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }
}
