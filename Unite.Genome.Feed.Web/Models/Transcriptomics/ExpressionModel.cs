using Unite.Genome.Feed.Data.Models.Enums;

namespace Unite.Genome.Feed.Web.Models.Transcriptomics;

public class ExpressionModel
{
    private string _geneId;
    private string _geneSymbol;
    private string _transcriptId;
    private string _transcriptSymbol;
    private string _source = "Ensembl";
    private int? _exonicLength;
    private int? _reads;


    public string GeneId { get => ParseId(_geneId)?.Trim(); set => _geneId = value; }
    public string GeneSymbol { get => _geneSymbol?.Trim(); set => _geneSymbol = value; }
    public string TranscriptId { get => ParseId(_transcriptId)?.Trim(); set => _transcriptId = value; }
    public string TranscriptSymbol { get => _transcriptSymbol?.Trim(); set => _transcriptSymbol = value; }
    public string Source { get => _source?.Trim(); set => _source = value; }
    public int? ExonicLength { get => _exonicLength; set => _exonicLength = value; }
    public int? Reads { get => _reads; set => _reads = value; }


    /// <summary>
    /// Retrieves expression data type: gene id (1), gene symbol (2), transcript id (3), transcript symbol (4), location (5), Undefined(0).
    /// </summary>
    /// <returns>Type of the expression data.</returns>
    public int GetDataType()
    {
        return !string.IsNullOrEmpty(GeneId) ? 1
             : !string.IsNullOrEmpty(GeneSymbol) ? 2
             : !string.IsNullOrEmpty(TranscriptId) ? 3
             : !string.IsNullOrEmpty(TranscriptSymbol) ? 4
             : 0;
    }

    /// <summary>
    /// Retrieves expression data corresponding to data type.
    /// </summary>
    /// <returns>Expression data.</returns>
    public KeyValuePair<string, (int Reads, int? Length)> GetData()
    {
        var dataType = GetDataType();

        return dataType switch
        {
            1 => new(GeneId, (Reads.Value, ExonicLength)),
            2 => new(GeneSymbol, (Reads.Value, ExonicLength)),
            3 => new(TranscriptId, (Reads.Value, ExonicLength)),
            4 => new(TranscriptSymbol, (Reads.Value, ExonicLength)),
            _ => default
        };
    }

    public string ParseId(string id)
    {
        return id?.Split('.')[0];
    }
}
