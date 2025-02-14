using Unite.Data.Entities.Genome.Analysis.Dna.Ssm;
using Unite.Data.Helpers.Genome.Dna.Ssm;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Genome.Variants;

namespace Unite.Genome.Indices.Services.Mappers;

public class SsmIndexMapper : VariantIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Variant entity) where T : SsmIndex, new()
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
    public static void Map(in Variant entity, SsmIndex index)
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
        index.Ref = entity.Ref;
        index.Alt = entity.Alt;

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
            Distance = entity.Distance,
            ProteinChange = ProteinChangeCodeGenerator.Generate(entity.ProteinStart, entity.ProteinEnd, entity.ProteinChange),
            CodonChange = CodonChangeCodeGenerator.Generate(entity.CDSStart, entity.CDSEnd, entity.CodonChange)
        };
    }
}
