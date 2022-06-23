using System.Text;

namespace Unite.Genome.Annotations.Data.Models.Audit;

public class AnnotationsUploadAudit
{
    public int GenesCreated;
    public int TranscriptsCreated;
    public int ProteinsCreated;
    public int AffectedTranscriptsCreated;

    public HashSet<long> Mutations;

    public AnnotationsUploadAudit()
    {
        Mutations = new HashSet<long>();
    }

    public override string ToString()
    {
        var message = new StringBuilder();

        message.AppendLine($"{Mutations.Count} mutations updated");
        message.AppendLine($"{GenesCreated} genes created");
        message.AppendLine($"{TranscriptsCreated} transcripts created");
        message.AppendLine($"{ProteinsCreated} proteins created");
        message.AppendLine($"{AffectedTranscriptsCreated} affected transcripts created");

        return message.ToString();
    }
}
