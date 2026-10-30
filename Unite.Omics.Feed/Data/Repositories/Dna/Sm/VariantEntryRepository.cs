using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm.Enums;
using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Omics.Feed.Data.Models.Dna.Sm;
using Unite.Essentials.Extensions;

namespace Unite.Omics.Feed.Data.Repositories.Dna.Sm;

public class VariantEntryRepository : VariantEntryRepository<VariantEntry, Variant, VariantModel>
{
    public VariantEntryRepository(DomainDbContext dbContext, VariantRepository<Variant, VariantModel> variantRepository) : base(dbContext, variantRepository)
    {
    }


    /// <summary>
    /// Removes all variant entries of given types associated with the specified sample.
    /// </summary>
    /// <param name="sampleId">Sample identifier.</param>
    /// <param name="types">Variant types to remove. If null, all types are removed.</param>
    /// <returns>Variant identifiers of the removed entries.</returns>
    public int[] RemoveAll(int sampleId, SmType[] types = null)
    {
        var query = _dbContext.Set<VariantEntry>()
            .Where(entity => entity.SampleId == sampleId);

        if (types.IsNotEmpty())
            query = query.Where(entity => types.Contains(entity.Entity.TypeId));

        var ids = query.Select(entity => entity.EntityId).ToArray();

        query.ExecuteDelete();

        return ids;
    }
}
