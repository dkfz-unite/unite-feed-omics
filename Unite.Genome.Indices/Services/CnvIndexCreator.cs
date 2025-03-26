using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Genome.Analysis.Dna.Cnv;
using Unite.Essentials.Extensions;
using Unite.Genome.Indices.Services.Mappers;
using Unite.Indices.Entities.Variants;

namespace Unite.Genome.Indices.Services;

public class CnvIndexCreator : VariantIndexCreator<Variant, VariantEntry>
{
    public CnvIndexCreator(VariantIndexingCache<Variant, VariantEntry> cache) : base(cache)
    {
    }


    public CnvIndex CreateIndex(object key)
    {
        var variantId = (int)key;

        return CreateVariantIndex(variantId);
    }


    private CnvIndex CreateVariantIndex(int variantId)
    {
        var variant = LoadVariant(variantId);

        if (variant == null)
            return null;

        return CreateVariantIndex(variant);
    }

    private CnvIndex CreateVariantIndex(Variant variant)
    {
        var index = CnvIndexMapper.CreateFrom<CnvIndex>(variant);

        index.Specimens = CreateSpecimenIndices(variant.Id);

        // If variant doesn't affect any specimens it should be removed.
        if (index.Specimens.IsEmpty())
            return null;

        index.Similars = CreateSimilarIndices(variant.Id);
        index.Stats = CreateStatsIndex(variant.Id);
        index.Data = CreateDataIndex(variant.Id);

        index.Stats.Donors = index.Similars?.Length ?? 1;

        return index;
    }

    private Variant LoadVariant(int variantId)
    {
        return _cache.Variants.FirstOrDefault(variant => variant.Id == variantId);
    }


    private Unite.Indices.Entities.Basic.Genome.Variants.VariantNavIndex[] CreateSimilarIndices(int variantId)
    {
        var variants = LoadSimilarVariants(variantId);

        return variants.Select(CreateSimilarIndex).ToArrayOrNull();
    }

    private Unite.Indices.Entities.Basic.Genome.Variants.VariantNavIndex CreateSimilarIndex(Variant variant)
    {
        return VariantNavIndexMapper.CreateFrom(variant);
    }

    private Variant[] LoadSimilarVariants(int variantId)
    {
        using var dbContext = _cache.DbContextFactory.CreateDbContext();

        var variantIds = _variantsRepository.GetSimilarVariants<Variant>([variantId]).Result;

        return dbContext.Set<Variant>()
            .AsNoTracking()
            .Where(entity => variantIds.Contains(entity.Id))
            .ToArray();
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
