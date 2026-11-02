namespace Unite.Omics.Feed.Data.Writers.Dna;

public class AnalysisWriteAudit : DataWriteAudit
{
    public int VariantsCreated;
    public int EntriesCreated;

    // New variants requiring annotation.
    public HashSet<int> Variants = [];

    // Affected variant entity IDs or CNVP profile IDs, including removed IDs.
    public HashSet<int> Entries = [];


    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            base.ToString(),
            $"{VariantsCreated} variants created",
            $"{EntriesCreated} entries created"
        ]);
    }
}
