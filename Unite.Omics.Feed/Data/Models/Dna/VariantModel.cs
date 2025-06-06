using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Data.Models.Dna;

public class VariantModel
{
    public int? Id;
    public Chromosome Chromosome;
    public int Start;
    public int End;
    public int? Length;
}
