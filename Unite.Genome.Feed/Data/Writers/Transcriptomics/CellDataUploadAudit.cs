namespace Unite.Genome.Feed.Data.Writers.Transcriptomics;

public class CellDataUploadAudit
{
    public int ResourcesCreated;

    public HashSet<int> Samples = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            $"{ResourcesCreated} resources created"
        ]);
    }
}
