using Unite.Data.Entities.Mutations;
using Unite.Data.Extensions;
using Unite.Indices.Entities.Basic.Mutations;

namespace Unite.Mutations.Indices.Services.Mappers
{
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
}
