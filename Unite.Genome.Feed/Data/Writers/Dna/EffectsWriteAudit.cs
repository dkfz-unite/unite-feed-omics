using Unite.Genome.Feed.Data.Writers;

namespace Unite.Genome.Feed.Data.Models;

public class EffectsWriteAudit : DataWriteAudit
{
    public int GenesCreated;
    public int TranscriptsCreated;
    public int ProteinsCreated;
    public int AffectedTranscriptsCreated;

    public HashSet<int> Variants = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            $"{Variants.Count} variants updated",
            $"{GenesCreated} genes created",
            $"{TranscriptsCreated} transcripts created",
            $"{ProteinsCreated} proteins created",
            $"{AffectedTranscriptsCreated} affected transcripts created",
            base.ToString()
        ]);
    }
}
