using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Genes;

namespace Unite.Omics.Indices.Services;

public class GeneExpressionIndexCreator
{
    private readonly GenesIndexingCache _cache;


    public GeneExpressionIndexCreator(GenesIndexingCache cache)
    {
        _cache = cache;
    }


    public GeneExpressionIndex[] CreateIndices(int geneId)
    {
        var geneExpressions = _cache.GeneExpressions.Where(entry => entry.EntityId == geneId).ToArray();

        if (geneExpressions.Length == 0)
            return null;

        var indices = new List<GeneExpressionIndex>();

        foreach (var geneExpression in geneExpressions)
        {
            var sample = _cache.Samples.FirstOrDefault(sample => sample.Id == geneExpression.SampleId);
            var specimen = _cache.Specimens.FirstOrDefault(specimen => specimen.Id == sample?.SpecimenId);

            if (specimen != null)
            {
                var index = new GeneExpressionIndex
                {
                    Id = $"{specimen.Id}_{geneId}",
                    Gene = new Unite.Indices.Entities.Basic.Omics.GeneNavIndex { Id = geneId },
                    Specimen = new Unite.Indices.Entities.Basic.Specimens.SpecimenNavIndex { Id = specimen.Id, ReferenceId = specimen.ReferenceId, Type = specimen.TypeId.ToDefinitionString() },
                    Expression = geneExpression.FPKM
                };

                indices.Add(index);
            }
        }

        return indices.ToArray();
    }
}
