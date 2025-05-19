using Unite.Data.Entities.Omics.Analysis.Dna.Cnv;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Omics.Variants;

namespace Unite.Omics.Indices.Services.Mappers;

public class CnvIndexMapper : VariantIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Variant entity) where T : CnvIndex, new()
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
    public static void Map(in Variant entity, CnvIndex index)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
        index.Chromosome = entity.ChromosomeId.ToDefinitionString();
        index.Start = entity.Start;
        index.End = entity.End;
        index.Length = entity.Length.Value;
        index.Type = entity.TypeId.ToDefinitionString();
        index.Loh = entity.Loh;
        index.Del = entity.Del;
        index.C1Mean = entity.C1Mean;
        index.C2Mean = entity.C2Mean;
        index.TcnMean = entity.TcnMean;
        index.C1 = entity.C1;
        index.C2 = entity.C2;
        index.Tcn = entity.Tcn;

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

    private static AffectedTranscriptIndex CreateFrom(in AffectedTranscript entity)
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
