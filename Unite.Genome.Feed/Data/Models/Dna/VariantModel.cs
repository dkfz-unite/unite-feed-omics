using Unite.Data.Entities.Genome.Enums;

namespace Unite.Genome.Feed.Data.Models.Dna;

public class VariantModel
{
    public int? Id { get; set; }
    public Chromosome Chromosome { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public int? Length { get; set; }
}
