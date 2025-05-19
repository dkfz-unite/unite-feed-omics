using Unite.Data.Entities.Omics.Analysis.Dna;
using Unite.Indices.Entities.Basic.Omics.Variants;

namespace Unite.Omics.Indices.Services.Mappers;

public class VariantNavIndexMapper
{
    /// <summary>
    /// Creates an index from the entity. Returns null if entity is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <typeparam name="T">Type of the index.</typeparam>
    /// <returns>Index created from the entity.</returns>
    public static VariantNavIndex CreateFrom(in Variant entity)
    {
        if (entity == null)
        {
            return null;
        }

        var index = new VariantNavIndex();

        Map(entity, index);

        return index;
    }

    /// <summary>
    /// Maps entity to index. Does nothing if either entity or index is null.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="index">Index.</param>
    public static void Map(in Variant entity, VariantNavIndex index)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Id = entity.Id;
    }
}
