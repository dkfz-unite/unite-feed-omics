using System.Text.Json.Serialization;
using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Annotations.Clients.Ensembl.Resources.Liftover;

public class SmResource
{
    [JsonPropertyName("chromosome")]
    public Chromosome? Chromosome { get; set; }

    [JsonPropertyName("start")]
    public int? Start { get; set; }

    [JsonPropertyName("end")]
    public int? End { get; set; }

    [JsonPropertyName("ref")]
    public string Ref { get; set; }

    [JsonPropertyName("alt")]
    public string Alt { get; set; }
}
