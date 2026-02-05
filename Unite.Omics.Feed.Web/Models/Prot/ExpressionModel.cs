using System.Text.Json.Serialization;
using Unite.Essentials.Tsv.Attributes;

namespace Unite.Omics.Feed.Web.Models.Prot;

public record class ExpressionModel
{
    private string _geneId;
    private string _geneSymbol;
    private string _transcriptId;
    private string _transcriptSymbol;
    private string _proteinId;
    private string _proteinAccession;
    private string _proteinSymbol;
    private double? _intensity;


    [JsonPropertyName("gene_id")]
    [Column("gene_id")]
    public string GeneId { get => ParseId(_geneId)?.Trim(); set => _geneId = value; }

    [JsonPropertyName("gene_symbol")]
    [Column("gene_symbol")]
    public string GeneSymbol { get => _geneSymbol?.Trim(); set => _geneSymbol = value; }

    [JsonPropertyName("transcript_id")]
    [Column("transcript_id")]
    public string TranscriptId { get => ParseId(_transcriptId)?.Trim(); set => _transcriptId = value; }

    [JsonPropertyName("transcript_symbol")]
    [Column("transcript_symbol")]
    public string TranscriptSymbol { get => _transcriptSymbol?.Trim(); set => _transcriptSymbol = value; }

    [JsonPropertyName("protein_id")]
    [Column("protein_id")]
    public string ProteinId { get => ParseId(_proteinId)?.Trim(); set => _proteinId = value; }

    [JsonPropertyName("protein_accession")]
    [Column("protein_accession")]
    public string ProteinAccession { get => _proteinAccession?.Trim(); set => _proteinAccession = value; }

    [JsonPropertyName("protein_symbol")]
    [Column("protein_symbol")]
    public string ProteinSymbol { get => _proteinSymbol?.Trim(); set => _proteinSymbol = value; }

    [JsonPropertyName("intensity")]
    [Column("intensity")]
    public double? Intensity { get => _intensity; set => _intensity = value; }

    /// <summary>
    /// Gets the type of the protein identification (1: gene id, 2: gene symbol, 3: transcript id, 4: transcript symbol, 5: protein id, 6: protein accession, 7: protein symbol, 0: undefined).
    /// </summary>
    /// <returns>Type of the protein identification.</returns>
    public byte GetKeyType()
    {
        return !string.IsNullOrEmpty(GeneId) ? (byte)1
             : !string.IsNullOrEmpty(GeneSymbol) ? (byte)2
             : !string.IsNullOrEmpty(TranscriptId) ? (byte)3
             : !string.IsNullOrEmpty(TranscriptSymbol) ? (byte)4
             : !string.IsNullOrEmpty(ProteinId) ? (byte)5
             : !string.IsNullOrEmpty(ProteinAccession) ? (byte)6
             : !string.IsNullOrEmpty(ProteinSymbol) ? (byte)7
             : (byte)0;
    }

    private static string ParseId(string id)
    {
        return id?.Split('.')[0];
    }
}
