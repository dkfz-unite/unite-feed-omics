using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Data.Models;

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
