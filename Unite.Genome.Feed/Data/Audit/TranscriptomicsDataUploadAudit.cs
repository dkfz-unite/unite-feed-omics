using System.Text;

namespace Unite.Genome.Feed.Data.Audit;

public class TranscriptomicsDataUploadAudit
{
    public int GenesCreated;
    public int ExpressionsAssociated;

    public HashSet<int> Genes = new HashSet<int>();

    public override string ToString()
    {
        var messages = new List<string>();

        messages.Add($"{GenesCreated} genes created");
        messages.Add($"{ExpressionsAssociated} gene expression associations created");

        return string.Join(Environment.NewLine, messages);
    }
}
