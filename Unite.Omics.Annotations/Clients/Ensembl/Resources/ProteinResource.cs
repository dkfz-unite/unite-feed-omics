using System.Text.Json.Serialization;
using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Annotations.Clients.Ensembl.Resources;

public record ProteinResource : LookupResource
{
    [JsonPropertyName("transcriptId")]
    public string TranscriptId { get; set; }

    [JsonPropertyName("accession")]
    public string Accession { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("database")]
    public string Database { get; set; }

    [JsonPropertyName("chromosome")]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public Chromosome Chromosome { get; set; }

    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("end")]
    public int End { get; set; }

    [JsonPropertyName("strand")]
    public bool Strand { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("isCanonical")]
    public bool IsCanonical { get; set; }


    [JsonPropertyName("features")]
    public ProteinFeatureResource[] Features { get; set; }
}
