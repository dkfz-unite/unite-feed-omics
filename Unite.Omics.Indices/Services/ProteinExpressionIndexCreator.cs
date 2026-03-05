using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Proteins;

namespace Unite.Omics.Indices.Services;

public class ProteinExpressionIndexCreator
{
    private readonly ProteinsIndexingCache _cache;


    public ProteinExpressionIndexCreator(ProteinsIndexingCache cache)
    {
        _cache = cache;
    }


    public ProteinExpressionIndex[] CreateIndices(int proteinId)
    {
        var proteinExpressions = _cache.ProteinExpressions.Where(entry => entry.EntityId == proteinId).ToArray();

        if (proteinExpressions.Length == 0)
            return null;

        var indices = new List<ProteinExpressionIndex>();

        foreach (var proteinExpression in proteinExpressions)
        {
            var sample = _cache.Samples.FirstOrDefault(sample => sample.Id == proteinExpression.SampleId);
            var specimen = _cache.Specimens.FirstOrDefault(specimen => specimen.Id == sample?.SpecimenId);

            if (specimen != null)
            {
                var index = new ProteinExpressionIndex
                {
                    Id = $"{specimen.Id}_{proteinId}",
                    Protein = new Unite.Indices.Entities.Basic.Omics.ProteinNavIndex { Id = proteinId },
                    Specimen = new Unite.Indices.Entities.Basic.Specimens.SpecimenNavIndex { Id = specimen.Id, ReferenceId = specimen.ReferenceId, Type = specimen.TypeId.ToDefinitionString() },
                    Expression = proteinExpression.Normalized
                };

                indices.Add(index);
            }
        }

        return indices.ToArray();
    }
}
