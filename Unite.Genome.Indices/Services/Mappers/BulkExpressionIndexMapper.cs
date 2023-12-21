using Unite.Data.Entities.Genome.Transcriptomics;
using Unite.Indices.Entities.Genes;

namespace Unite.Genome.Indices.Services.Mappers;

public class BulkExpressionIndexMapper
{
    public static BulkExpressionIndex CreateFrom(in BulkExpression entity)
    {
        if (entity == null)
        {
            return null;
        }

        var index = new BulkExpressionIndex();

        Map(entity, index);

        return index;
    }

    public static void Map(in BulkExpression entity, BulkExpressionIndex index)
    {
        if (entity == null || index == null)
        {
            return;
        }

        index.Reads = entity.Reads;
        index.Tpm = entity.TPM;
        index.Fpkm = entity.FPKM;
    }
}
