using System.Text.Json.Serialization;

namespace Unite.Omics.Annotations.Clients.Ensembl.Resources.Vep;

internal record AnnotatedVariantResource
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("input")]
    public string Input { get; set; }

    [JsonPropertyName("transcript_consequences")]
    public AffectedTranscriptResource[] AffectedTranscripts { get; set; }

    [JsonIgnore]
    public int VariantId => int.Parse(Id.Split(".").First());
}
