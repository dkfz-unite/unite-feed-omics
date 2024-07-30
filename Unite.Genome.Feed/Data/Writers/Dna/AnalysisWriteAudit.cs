namespace Unite.Genome.Feed.Data.Writers.Dna;

public class AnalysisWriteAudit : DataWriteAudit
{
    public int SsmsCreated;
    public int SsmsAssociated;
    public int CnvsCreated;
    public int CnvsAssociated;
    public int SvsCreated;
    public int SvsAssociated;

    public HashSet<int> Ssms = [];
    public HashSet<int> SsmsEntries = [];
    public HashSet<int> Cnvs = [];
    public HashSet<int> CnvsEntries = [];
    public HashSet<int> Svs = [];
    public HashSet<int> SvsEntries = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            base.ToString(),
            $"{SsmsCreated} SSMs created",
            $"{SsmsAssociated} SSMs associated",
            $"{CnvsCreated} CNVs created",
            $"{CnvsAssociated} CNVs associated",
            $"{SvsCreated} SVs created",
            $"{SvsAssociated} SVs associated"
        ]);
    }
}
