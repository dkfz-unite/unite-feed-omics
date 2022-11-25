namespace Unite.Genome.Annotations.Data.Models;

public class ProteinModel
{
    public string EnsemblId { get; set; }

    public int Start { get; set; }
    public int End { get; set; }
    public int Length { get; set; }
    public bool IsAnnotated { get; set; }
}
