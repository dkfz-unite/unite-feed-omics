namespace Unite.Omics.Feed.Data.Writers.Dna;

public class AnalysisWriteAudit : DataWriteAudit
{
    public int SmsCreated;
    public int SmsAssociated;
    public int CnvsCreated;
    public int CnvsAssociated;
    public int SvsCreated;
    public int SvsAssociated;
    public int CnvProfilesCreatedCount = 0;
    public int CnvProfilesUpdatedCount = 0;

    public HashSet<int> Sms = [];
    public HashSet<int> SmsEntries = [];
    public HashSet<int> Cnvs = [];
    public HashSet<int> CnvsEntries = [];
    public HashSet<int> Svs = [];
    public HashSet<int> SvsEntries = [];
    public ISet<int> CnvProfilesCreated = new HashSet<int>();
    public ISet<int> CnvProfilesUpdated = new HashSet<int>();

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            base.ToString(),
            $"{SmsCreated} SMs created",
            $"{SmsAssociated} SMs associated",
            $"{CnvsCreated} CNVs created",
            $"{CnvsAssociated} CNVs associated",
            $"{SvsCreated} SVs created",
            $"{SvsAssociated} SVs associated",
            $"{CnvProfilesCreatedCount} CNV Profiles created",
            $"{CnvProfilesUpdatedCount} CNV Profiles updated",
        ]);
    }
}
