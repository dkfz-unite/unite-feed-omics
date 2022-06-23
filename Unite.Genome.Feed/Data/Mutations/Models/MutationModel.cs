using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Mutations.Enums;

namespace Unite.Genome.Feed.Data.Mutations.Models;

public class MutationModel
{
    public string Code { get; set; }
    public Chromosome Chromosome { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public MutationType Type { get; set; }
    public string ReferenceBase { get; set; }
    public string AlternateBase { get; set; }
}
