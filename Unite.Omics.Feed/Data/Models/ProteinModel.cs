namespace Unite.Omics.Feed.Data.Models;

public class ProteinModel
{
    public string Id;
    public int Start;
    public int End;
    public int Length;
    public bool IsCanonical;

    public TranscriptModel Transcript;
}
