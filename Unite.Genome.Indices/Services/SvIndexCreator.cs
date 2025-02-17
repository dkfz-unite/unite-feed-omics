using Unite.Data.Entities.Genome.Analysis.Dna.Sv;
using Unite.Essentials.Extensions;
using Unite.Genome.Indices.Services.Mappers;
using Unite.Indices.Entities.Variants;

namespace Unite.Genome.Indices.Services;

public class SvIndexCreator : VariantIndexCreator<Variant, VariantEntry>
{
    public SvIndexCreator(VariantIndexingCache<Variant, VariantEntry> cache) : base(cache)
    {
    }


    public SvIndex CreateIndex(object key)
    {
        var variantId = (int)key;

        return CreateVariantIndex(variantId);
    }


    private SvIndex CreateVariantIndex(int variantId)
    {
        var variant = LoadVariant(variantId);

        if (variant == null)
            return null;

        return CreateVariantIndex(variant);
    }

    private SvIndex CreateVariantIndex(Variant variant)
    {
        var index = SvIndexMapper.CreateFrom<SvIndex>(variant);

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
