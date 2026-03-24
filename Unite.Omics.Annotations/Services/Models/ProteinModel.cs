using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Annotations.Services.Models;

public class ProteinModel
{
    public int? Id;

    public string StableId;
    public string Accession;
    public string Symbol;
    public string Description;
    public string Database;
    public Chromosome Chromosome;
    public int Start;
    public int End;
    public bool? Strand;
    public int Length;
    public bool IsCanonical;

    public TranscriptModel Transcript;
}
