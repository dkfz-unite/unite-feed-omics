using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Feed.Data.Models;

public class ProteinModel
{
    public string Id;
    public string Accession;
    public string Symbol;
    public string Description;
    public string Database;
    public Chromosome Chromosome;
    public ChromosomeArm? ChromosomeArm;
    public int Start;
    public int End;
    public bool? Strand;
    public int Length;
    public bool IsCanonical;

    public TranscriptModel Transcript;
}
