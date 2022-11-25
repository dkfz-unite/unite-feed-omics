using Unite.Data.Entities.Genome.Enums;

namespace Unite.Genome.Annotations.Data.Models;

public class GeneModel
{
    public string EnsemblId { get; set; }

    public string Symbol { get; set; }
    public string Biotype { get; set; }
    public Chromosome Chromosome { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public bool Strand { get; set; }
    public bool IsAnnotated { get; set; }
}
