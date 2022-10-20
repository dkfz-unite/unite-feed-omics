using Unite.Data.Entities.Genome.Enums;

namespace Unite.Genome.Feed.Data.Models.Variants;

public abstract class VariantModel
{
    public Chromosome Chromosome { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
}
