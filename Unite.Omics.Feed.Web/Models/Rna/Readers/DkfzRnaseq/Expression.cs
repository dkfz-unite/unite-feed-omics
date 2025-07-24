using Unite.Essentials.Tsv.Attributes;

namespace Unite.Omics.Feed.Web.Models.Rna.Readers.DkfzRnaseq;

public class Expression
{
    // TSV header starts from '#' which has to be ignored while reading the file.
    
    [Column("gene_id")]
    public string GeneId { get; set; }

    [Column("exonic_length")]
    public int ExonicLength { get; set; }

    [Column("num_reads")]
    public int Reads { get; set; }


    public ExpressionModel Convert()
    {
        try
        {
            return new ExpressionModel
            {
                GeneId = GeneId,
                ExonicLength = ExonicLength,
                Reads = Reads
            };
        }
        catch
        {
            return null;
        }
    }
}
