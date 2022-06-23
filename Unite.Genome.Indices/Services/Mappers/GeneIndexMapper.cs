using Unite.Data.Entities.Genome;
using Unite.Data.Extensions;
using Unite.Indices.Entities.Basic.Genome;

namespace Unite.Genome.Indices.Services.Mappers;

public class GeneIndexMapper
{
    public void Map(in Gene gene, GeneIndex index)
    {
        index.Id = gene.Id;
        index.Symbol = gene.Symbol;
        index.Biotype = gene.Biotype?.Value;
        index.Chromosome = gene.ChromosomeId.ToDefinitionString();
        index.Start = gene.Start;
        index.End = gene.End;
        index.Strand = gene.Strand;

        index.EnsemblId = gene.Info?.EnsemblId;
    }
}
