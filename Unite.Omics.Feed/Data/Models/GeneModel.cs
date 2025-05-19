using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Data.Models;

public class GeneModel
{
    public string Id;
    public string Symbol;
    public string Description;
    public string Biotype;
    public Chromosome Chromosome;
    public int Start;
    public int End;
    public bool Strand;
    public int? ExonicLength;
}
