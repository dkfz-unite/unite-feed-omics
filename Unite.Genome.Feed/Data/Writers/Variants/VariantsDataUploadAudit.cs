namespace Unite.Genome.Feed.Data.Writers.Variants;

public class VariantsDataUploadAudit
{
    public int SsmsCreated;
    public int SsmsAssociated;
    public int CnvsCreated;
    public int CnvsAssociated;
    public int SvsCreated;
    public int SvsAssociated;

    public HashSet<long> Ssms = [];
    public HashSet<long> SsmsEntries = [];
    public HashSet<long> Cnvs = [];
    public HashSet<long> CnvsEntries = [];
    public HashSet<long> Svs = [];
    public HashSet<long> SvsEntries = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            $"{SsmsCreated} SSMs created",
            $"{SsmsAssociated} SSMs associations created",
            $"{CnvsCreated} CNVs created",
            $"{CnvsAssociated} CNVs associations created",
            $"{SvsCreated} SVs created",
            $"{SvsAssociated} SVs associations created"
        ]);
    }
}
