using Microsoft.EntityFrameworkCore;
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

        var types = new[] { Data.Entities.Genome.Analysis.Dna.Sv.Enums.SvType.ITX, Data.Entities.Genome.Analysis.Dna.Sv.Enums.SvType.CTX };
        var target = _cache.Variants.First(entity => entity.Id == variantId);
        
        if (types.Contains(target.TypeId))
        {
            var distance = 5000;
            var targetStartMin = Math.Max(0, target.End - distance);
            var targetStartMax = target.End + distance;
            var targetEndMin = Math.Max(0, target.OtherStart - distance);
            var targetEndMax = target.OtherStart + distance;

            return dbContext.Set<Variant>()
                .AsNoTracking()
                .Where(current => current.TypeId == target.TypeId && current.ChromosomeId == target.ChromosomeId && current.OtherChromosomeId == target.OtherChromosomeId)
                .Where(current => current.End >= targetStartMin && current.End <= targetStartMax && current.OtherStart >= targetEndMin && current.OtherStart <= targetEndMax)
                .ToArray();
        }
        else
        {
            var targetLength = target.OtherStart - target.End;
            var overlap = 0.9;
        
            return dbContext.Set<Variant>()
                .AsNoTracking()
                .Where(current => current.TypeId == target.TypeId)
                .Where(current => current.OtherEnd >= target.End && current.End <= target.OtherStart)
                .ToArray()
                .Where(current => {
                    var start = Math.Max(current.End, target.End);
                    var end = Math.Min(current.OtherStart, target.OtherStart);
                    var length = end - start;

                    var currentLength = current.OtherStart - current.End;
                    var currentOverlap = (double)length / currentLength;
                    var targetOverlap = (double)length / targetLength;
                
                    return Math.Min(currentOverlap, targetOverlap) >= overlap;
                })
                .ToArray();
        }
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
