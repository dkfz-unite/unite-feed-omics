using Unite.Data.Entities.Genome.Enums;

namespace Unite.Genome.Feed.Data.Models.Dna;

public class VariantModel
{
    public int? Id;
    public Chromosome Chromosome;
    public int Start;
    public int End;
    public int? Length;
}
