namespace Unite.Genome.Feed.Data.Writers.Rna;

public class AnalysisWriteAudit : DataWriteAudit
{
    public int GenesCreated;
    public int ExpressionsCreated;

    public HashSet<int> Genes = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            $"{GenesCreated} genes created",
            $"{ExpressionsCreated} gene expressions created",
            base.ToString()
        ]);
    }
}
