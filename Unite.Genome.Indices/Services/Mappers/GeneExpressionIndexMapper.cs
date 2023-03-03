using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Indices.Entities.Basic.Genome.Transcriptomics;

namespace Unite.Genome.Indices.Services.Mappers;

public class GeneExpressionIndexMapper
{
    public void Map(in GeneExpression entity, GeneExpressionIndex index)
    {
        index.Reads = entity.Reads;
        index.TPM = entity.TPM;
        index.FPKM = entity.FPKM;
    }
}
