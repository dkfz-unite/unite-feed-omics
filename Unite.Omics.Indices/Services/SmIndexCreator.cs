using Unite.Data.Entities.Omics.Analysis.Dna.Sm;
using Unite.Essentials.Extensions;
using Unite.Omics.Indices.Services.Mappers;
using Unite.Indices.Entities.Variants;

namespace Unite.Omics.Indices.Services;

public class SmIndexCreator : VariantIndexCreator<Variant, VariantEntry>
{
    public SmIndexCreator(VariantIndexingCache<Variant, VariantEntry> cache) : base(cache)
    {
    }


    public SmIndex CreateIndex(object key)
    {
        var variantId = (int)key;

        return CreateVariantIndex(variantId);
    }


    private SmIndex CreateVariantIndex(int variantId)
    {
        var variant = LoadVariant(variantId);

        if (variant == null)
            return null;

        return CreateVariantIndex(variant);
    }

    private SmIndex CreateVariantIndex(Variant variant)
    {
        var index = SmIndexMapper.CreateFrom<SmIndex>(variant);

        index.Specimens = CreateSpecimenIndices(variant.Id);

        // If variant doesn't affect any specimens it should be removed.
        if (index.Specimens.IsEmpty())
            return null;

        index.Stats = CreateStatsIndex(variant.Id);
        index.Data = CreateDataIndex(variant.Id);

        return index;
    }

    private Variant LoadVariant(int variantId)
    {
        return _cache.Variants.FirstOrDefault(variant => variant.Id == variantId);
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
