namespace Unite.Genome.Feed.Web.Models.Transcriptome;

public class ExpressionModel
{
    private string _geneSymbol;
    private string _geneSource;
    private double? _reads;
    private double? _TPM;
    private double? _FPKM;

    public string GeneSymbol { get => _geneSymbol?.Trim(); set => _geneSymbol = value; }
    public string GeneSource { get => _geneSource?.Trim(); set => _geneSource = value; }
    public double? Reads { get => _reads; set => _reads = value; }
    public double? TPM { get => _TPM; set => _TPM = value; }
    public double? FPKM { get => _FPKM; set => _FPKM = value; }
}


public class TranscriptExpression
{
    public int SampleId;
    public long Id;
    public int GeneId;
    public double Reads;
    public double FPKM;
}

public class TranscriptExpressionOccurrence
{
    public int SampleId;
    public int TranscriptExpressionId;

    public string GeneSymbol;
    public string GeneSymbolSource;
}