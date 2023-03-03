using Unite.Data.Entities.Genome.Enums;

namespace Unite.Genome.Annotations.Services.Models;

public class TranscriptModel
{
    public string Id;
    public string Symbol;
    public string Description;
    public string Biotype;
    public bool IsCanonical;
    public Chromosome Chromosome;
    public int Start;
    public int End;
    public bool Strand;
    public int? ExonicLength;

    public GeneModel Gene;
}
