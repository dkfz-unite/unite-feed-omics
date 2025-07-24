using Unite.Data.Entities.Omics.Enums;
using Unite.Essentials.Tsv.Attributes;

namespace Unite.Omics.Feed.Web.Models.Dna.Sm.Readers.Tsv;

public class Variant
{
    [Column("chromosome")]
    public Chromosome Chromosome { get; set; }

    [Column("position")]
    public string Position { get; set; }

    [Column("ref")]
    public string Ref { get; set; }

    [Column("alt")]
    public string Alt { get; set; }


    public VariantModel Convert()
    {
        return new VariantModel
        {
            Chromosome = Chromosome,
            Position = Position,
            Ref = Ref,
            Alt = Alt
        };
    }
}
