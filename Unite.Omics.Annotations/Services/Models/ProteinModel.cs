namespace Unite.Omics.Annotations.Services.Models;

public class ProteinModel
{
    public string Id;
    public string Accession;
    public string Symbol;
    public string Description;
    public string Database;
    public int Start;
    public int End;
    public int Length;
    public bool IsCanonical;

    public TranscriptModel Transcript;
}
