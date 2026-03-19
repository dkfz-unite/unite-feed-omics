using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Genes;

namespace Unite.Omics.Indices.Services;

public class GeneExpressionIndexEntityBuilder: IndexEntityBuilder<GeneExpressionIndex, GenesIndexingCache>
{
    public override GeneExpressionIndex[] Create(int key, GenesIndexingCache cache)
    {
        var geneExpressions = cache.GeneExpressions.Where(entry => entry.EntityId == key).ToArray();

        if (geneExpressions.Length == 0)
            return null;

        var indices = new List<GeneExpressionIndex>();

        foreach (var geneExpression in geneExpressions)
        {
            var sample = cache.Samples.FirstOrDefault(sample => sample.Id == geneExpression.SampleId);
            var specimen = cache.Specimens.FirstOrDefault(specimen => specimen.Id == sample?.SpecimenId);

            if (specimen != null)
            {
                var index = new GeneExpressionIndex
                {
                    Id = $"{specimen.Id}_{key}",
                    Gene = new Unite.Indices.Entities.Basic.Omics.GeneNavIndex { Id = key },
                    Specimen = new Unite.Indices.Entities.Basic.Specimens.SpecimenNavIndex { Id = specimen.Id, ReferenceId = specimen.ReferenceId, Type = specimen.TypeId.ToDefinitionString() },
                    Expression = geneExpression.Normalized
                };

                indices.Add(index);
            }
        }

        return indices.ToArray();
    }
}
