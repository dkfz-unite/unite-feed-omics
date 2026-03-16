namespace Unite.Omics.Feed.Data.Writers.Prot;

public class AnalysisWriteAudit : DataWriteAudit
{
    public int GenesCreated;
    public int TranscriptsCreated;
    public int ProteinsCreated;
    public int ExpressionsCreated;

    public HashSet<int> Proteins = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            base.ToString(),
            $"{GenesCreated} genes created",
            $"{ProteinsCreated} proteins created",
            $"{TranscriptsCreated} transcripts created",
            $"{ExpressionsCreated} protein expressions created"
        ]);
    }
}
