namespace Unite.Omics.Feed.Data.Writers.CnvProfile;

public class AnalysisWriteAudit: DataWriteAudit
{
    public int CnvProfilesCreated = 0;

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            base.ToString(),
            $"{CnvProfilesCreated} CMV profiles created"
        ]);
    }
}