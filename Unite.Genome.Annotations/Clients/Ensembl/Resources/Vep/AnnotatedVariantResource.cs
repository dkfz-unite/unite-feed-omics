using System.Text.Json.Serialization;

namespace Unite.Genome.Annotations.Clients.Ensembl.Resources.Vep;

internal record AnnotatedVariantResource
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("input")]
    public string Input { get; set; }

    [JsonPropertyName("transcript_consequences")]
    public AffectedTranscriptResource[] AffectedTranscripts { get; set; }

    [JsonIgnore]
    public long VariantId => long.Parse(Id.Split(".").First());
}
