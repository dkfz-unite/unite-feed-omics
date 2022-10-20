using System.Text;

namespace Unite.Genome.Annotations.Data.Models;

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
        var message = new StringBuilder();

        message.AppendLine($"{Variants.Count} variants updated");
        message.AppendLine($"{GenesCreated} genes created");
        message.AppendLine($"{TranscriptsCreated} transcripts created");
        message.AppendLine($"{ProteinsCreated} proteins created");
        message.AppendLine($"{AffectedTranscriptsCreated} affected transcripts created");

        return message.ToString();
    }
}
