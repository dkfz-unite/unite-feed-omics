using Unite.Data.Entities.Omics;
using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Omics;

namespace Unite.Omics.Indices.Services.Mappers;

public class ProteinIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static T CreateFrom<T>(in Protein entity) where T : ProteinIndex, new()
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
    public static void Map(in Protein entity, ProteinIndex index)
    {
        if (entity == null || index == null)
        {
            return;
        }
        
        index.Id = entity.Id;
        index.AccessionId = entity.AccessionId;
        index.StableId = entity.StableId;
        index.Symbol = entity.Symbol;
        index.Description = entity.Description;
        index.Database = entity.Database;
        index.IsCanonical = entity.IsCanonical;
        index.Chromosome = entity.ChromosomeId.ToDefinitionString();
        index.Start = entity.Start;
        index.End = entity.End;
        index.Strand = entity.Strand;
        index.Length = entity.Length;
    }
}
