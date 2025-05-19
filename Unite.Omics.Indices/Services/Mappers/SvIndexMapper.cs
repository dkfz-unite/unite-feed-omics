using Unite.Data.Entities.Omics.Analysis.Dna.Sv;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Omics.Variants;

namespace Unite.Omics.Indices.Services.Mappers;

public class SvIndexMapper : VariantIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Variant entity) where T : SvIndex, new()
    {
        if (entity == null)
        {
            return null;
        }

        var index = new T();

        Map(entity, index);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    public static void Map(in Variant entity, SvIndex index)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
        index.Chromosome = entity.ChromosomeId.ToDefinitionString();
        index.Start = entity.Start;
        index.End = entity.End;
        index.OtherChromosome = entity.OtherChromosomeId.ToDefinitionString();
        index.OtherStart = entity.OtherStart;
        index.OtherEnd = entity.OtherEnd;
        index.Length = entity.Length;
        index.Type = entity.TypeId.ToDefinitionString();
        index.Inverted = entity.Inverted;

        index.AffectedFeatures = CreateFrom(entity.AffectedTranscripts);
    }


    private static AffectedFeatureIndex[] CreateFrom(in IEnumerable<AffectedTranscript> entities)
    {
        if (entities?.Any() != true)
        {
            return null;
        }

        return entities.Select(entity =>
        {
            return new AffectedFeatureIndex
            {
                Gene = CreateFrom(entity.Feature?.Gene),
                Transcript = CreateFrom(entity),
                Effects = CreateFrom(entity.Effects)
            };

        }).ToArray();
    }

    protected static AffectedTranscriptIndex CreateFrom(in AffectedTranscript entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new AffectedTranscriptIndex
        {
            Feature = CreateFrom(entity.Feature),
            OverlapBpNumber = entity.OverlapBpNumber,
            OverlapPercentage = entity.OverlapPercentage,
            Distance = entity.Distance
        };
    }
}
