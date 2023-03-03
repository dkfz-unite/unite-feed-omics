namespace Unite.Genome.Feed.Data.Models;

public class ConsequencesDataUploadAudit
{
    public int GenesCreated;
    public int TranscriptsCreated;
    public int ProteinsCreated;
    public int AffectedTranscriptsCreated;

    public HashSet<long> Variants;

    public ConsequencesDataUploadAudit()
    {
        Variants = new HashSet<long>();
    }

    public override string ToString()
    {
        var messages = new List<string>();

        messages.Add($"{Variants.Count} variants updated");
        messages.Add($"{GenesCreated} genes created");
        messages.Add($"{TranscriptsCreated} transcripts created");
        messages.Add($"{ProteinsCreated} proteins created");
        messages.Add($"{AffectedTranscriptsCreated} affected transcripts created");

        return string.Join(Environment.NewLine, messages);
    }
}
