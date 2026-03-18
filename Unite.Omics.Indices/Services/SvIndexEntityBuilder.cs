using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services.Mappers;
using Unite.Indices.Entities.Variants;

namespace Unite.Omics.Indices.Services;

public class SvIndexEntityBuilder : VariantIndexEntityBuilder<Variant, VariantEntry, SvIndex>
{
    public override SvIndex[] Create(int key, VariantIndexingCache<Variant, VariantEntry> cache)
    {
        var index = CreateVariantIndex(key, cache);

        return index == null ? null : [index];
    }

    private SvIndex CreateVariantIndex(int variantId, VariantIndexingCache<Variant, VariantEntry> cache)
    {
        var variant = LoadVariant(variantId, cache);

        if (variant == null)
            return null;

        return CreateVariantIndex(variant, cache);
    }

    private SvIndex CreateVariantIndex(Variant variant, VariantIndexingCache<Variant, VariantEntry> cache)
    {
        var index = SvIndexMapper.CreateFrom<SvIndex>(variant);

        index.Specimens = CreateSpecimenIndices(variant.Id, cache);

        // If variant doesn't affect any specimens it should be removed.
        if (index.Specimens.IsEmpty())
            return null;

        index.Similars = CreateSimilarIndices(variant.Id, cache);
        index.Stats = CreateStatsIndex(variant.Id, cache);
        index.Data = CreateDataIndex(variant.Id, cache);

        index.Stats.Donors += index.Similars?.Length ?? 0;

        return index;
    }

    private Variant LoadVariant(int variantId, VariantIndexingCache<Variant, VariantEntry> cache)
    {
        return cache.Variants.FirstOrDefault(variant => variant.Id == variantId);
    }
    
    private Unite.Indices.Entities.Basic.Omics.Variants.VariantNavIndex[] CreateSimilarIndices(int variantId, VariantIndexingCache<Variant, VariantEntry> cache)
    {
        var variants = LoadSimilarVariants(variantId, cache);

        return variants.Select(CreateSimilarIndex).ToArrayOrNull();
    }

    private Unite.Indices.Entities.Basic.Omics.Variants.VariantNavIndex CreateSimilarIndex(Variant variant)
    {
        return VariantNavIndexMapper.CreateFrom(variant);
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
