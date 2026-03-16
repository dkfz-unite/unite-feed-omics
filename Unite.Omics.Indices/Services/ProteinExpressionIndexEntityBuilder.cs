using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Proteins;

namespace Unite.Omics.Indices.Services;

public class ProteinExpressionIndexEntityBuilder: IndexEntityBuilder<ProteinExpressionIndex, ProteinsIndexingCache>
{
    public override ProteinExpressionIndex[] Create(int key, ProteinsIndexingCache cache)
    {
        var proteinExpressions = cache.ProteinExpressions.Where(entry => entry.EntityId == key).ToArray();

        if (proteinExpressions.Length == 0)
            return null;

        var indices = new List<ProteinExpressionIndex>();

        foreach (var proteinExpression in proteinExpressions)
        {
            var sample = cache.Samples.FirstOrDefault(sample => sample.Id == proteinExpression.SampleId);
            var specimen = cache.Specimens.FirstOrDefault(specimen => specimen.Id == sample?.SpecimenId);

            if (specimen != null)
            {
                var index = new ProteinExpressionIndex
                {
                    Id = $"{specimen.Id}_{key}",
                    Protein = new Unite.Indices.Entities.Basic.Omics.ProteinNavIndex { Id = key },
                    Specimen = new Unite.Indices.Entities.Basic.Specimens.SpecimenNavIndex { Id = specimen.Id, ReferenceId = specimen.ReferenceId, Type = specimen.TypeId.ToDefinitionString() },
                    Expression = proteinExpression.Normalized
                };

                indices.Add(index);
            }
        }

        return indices.ToArray();
    }
}
