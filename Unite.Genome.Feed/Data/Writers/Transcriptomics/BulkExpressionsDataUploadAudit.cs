namespace Unite.Genome.Feed.Data.Writers.Transcriptomics;

public class BulkExpressionsDataUploadAudit
{
    public int GenesCreated;
    public int ExpressionsAssociated;

    public HashSet<int> Genes = [];

    public override string ToString()
    {
        return string.Join(Environment.NewLine,
        [
            $"{GenesCreated} genes created",
            $"{ExpressionsAssociated} gene expression associations created"
        ]);
    }
}
