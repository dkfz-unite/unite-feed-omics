using System.Text.Json.Serialization;

namespace Unite.Omics.Annotations.Clients.Ensembl.Resources;

public record ProteinResource : LookupResource
{
    [JsonPropertyName("transcriptId")]
    public string TranscriptId { get; set; }

    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("end")]
    public int End { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("isCanonical")]
    public bool IsCanonical { get; set; }


    [JsonPropertyName("features")]
    public ProteinFeatureResource[] Features { get; set; }
}
