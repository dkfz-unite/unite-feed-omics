using System.Text.Json.Serialization;
using Unite.Essentials.Tsv.Attributes;

namespace Unite.Omics.Feed.Web.Models.Prot;

public record class ExpressionModel
{
    private string _id;
    private string _accession;
    private string _symbol;
    private double? _intensity;


    [JsonPropertyName("id")]
    [Column("id")]
    public string Id { get => ParseId(_id)?.Trim(); set => _id = value; }

    [JsonPropertyName("accession")]
    [Column("accession")]
    public string Accession { get => _accession?.Trim(); set => _accession = value; }

    [JsonPropertyName("symbol")]
    [Column("symbol")]
    public string Symbol { get => _symbol?.Trim(); set => _symbol = value; }

    [JsonPropertyName("intensity")]
    [Column("intensity")]
    public double? Intensity { get => _intensity; set => _intensity = value; }


    /// <summary>
    /// Retrieves key type of the expression : id (1), accession (2), symbol (3) or undefined(0).
    /// </summary>
    /// <returns>Expression key type.</returns>
    public byte GetKeyType()
    {
        return !string.IsNullOrEmpty(Id) ? (byte)1
             : !string.IsNullOrEmpty(Accession) ? (byte)2
             : !string.IsNullOrEmpty(Symbol) ? (byte)3
             : (byte)0;
    }

    /// <summary>
    /// Retrieves expression data corresponding to the key type.
    /// </summary>
    /// <returns>Expression data.</returns>
    public KeyValuePair<string, double> GetData()
    {
        var keyType = GetKeyType();
        var intensity = Intensity.Value;

        return keyType switch
        {
            1 => new KeyValuePair<string, double>(Id, intensity),
            2 => new KeyValuePair<string, double>(Accession, intensity),
            3 => new KeyValuePair<string, double>(Symbol, intensity),
            _ => default
        }; 
    }


    private static string ParseId(string id)
    {
        return id?.Split('.')[0];
    }
}
