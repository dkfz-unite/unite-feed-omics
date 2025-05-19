using System.Text.Json.Serialization;

namespace Unite.Omics.Annotations.Clients.Ensembl.Resources;

public record ReferenceResource
{
    [JsonPropertyName("primary_id")]
    public string Id { get; set; }

    [JsonPropertyName("display_id")]
    public string Symbol { get; set; }

    [JsonPropertyName("description")]
    public string Name { get; set; }

    [JsonPropertyName("dbname")]
    public string Database { get; set; }
}
