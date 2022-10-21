using System.Text.Json.Serialization;

namespace Unite.Genome.Annotations.Clients.Ensembl.Resources;

public class ProteinResource : LookupResource
{
    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("end")]
    public int End { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }
}
