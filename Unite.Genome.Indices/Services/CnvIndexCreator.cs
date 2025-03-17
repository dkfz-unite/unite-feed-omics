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

        var variant = _cache.Variants.First(entity => entity.Id == variantId);

        var target = variant;
        var targetLength = variant.End - variant.Start;
        var overlap = 0.9;
        
        return dbContext.Set<Variant>()
            .AsNoTracking()
            .Where(current => current.TypeId == target.TypeId && current.Loh == target.Loh && current.Del == target.Del)
            .Where(current => current.End >= target.Start && current.Start <= target.End)
            .ToArray()
            .Where(current => {
                var start = Math.Max(current.Start, target.Start);
                var end = Math.Min(current.End, target.End);
                var length = end - start;

                var currentLength = current.End - current.Start;
                var currentOverlap = (double)length / currentLength;
                var targetOverlap = (double)length / targetLength;
                
                return Math.Min(currentOverlap, targetOverlap) >= overlap;
            })
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
