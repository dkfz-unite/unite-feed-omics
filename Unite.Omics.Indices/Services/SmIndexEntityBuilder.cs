using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services.Mappers;
using Unite.Indices.Entities.Variants;

namespace Unite.Omics.Indices.Services;

public class SmIndexEntityBuilder : VariantIndexEntityBuilder<Variant, VariantEntry, SmIndex>
{
    public override SmIndex[] Create(int key, VariantIndexingCache<Variant, VariantEntry> cache)
    {
        return [CreateVariantIndex(key, cache)];
    }
    
    private SmIndex CreateVariantIndex(int variantId, VariantIndexingCache<Variant, VariantEntry> cache)
    {
        var variant = LoadVariant(variantId, cache);

        if (variant == null)
            return null;

        return CreateVariantIndex(variant, cache);
    }

    private SmIndex CreateVariantIndex(Variant variant, VariantIndexingCache<Variant, VariantEntry> cache)
    {
        var index = SmIndexMapper.CreateFrom<SmIndex>(variant);

        index.Specimens = CreateSpecimenIndices(variant.Id, cache);

        // If variant doesn't affect any specimens it should be removed.
        if (index.Specimens.IsEmpty())
            return null;

        index.Stats = CreateStatsIndex(variant.Id, cache);
        index.Data = CreateDataIndex(variant.Id, cache);

        return index;
    }

    private Variant LoadVariant(int variantId, VariantIndexingCache<Variant, VariantEntry> cache)
    {
        return cache.Variants.FirstOrDefault(variant => variant.Id == variantId);
    }


    protected override int[] GetAffectedGenes(Variant variant)
    {
        return variant.AffectedTranscripts?
            .Where(effect => effect.Feature.GeneId != null)
            .Select(effect => effect.Feature.GeneId.Value)
            .Distinct()
            .ToArray();
    }
}
