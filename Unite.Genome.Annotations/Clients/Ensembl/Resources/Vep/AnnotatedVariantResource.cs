using System.Text.Json.Serialization;

namespace Unite.Genome.Annotations.Clients.Ensembl.Resources.Vep;

internal record AnnotatedVariantResource
{
    private AffectedTranscriptResource[] _affectedTranscripts;

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("input")]
    public string Input { get; set; }

    [JsonPropertyName("transcript_consequences")]
    public AffectedTranscriptResource[] AffectedTranscripts
    {
        get => GetAffectedTranscripts(_affectedTranscripts);
        set => _affectedTranscripts = value;
    }

    [JsonIgnore]
    public long VariantId => long.Parse(Id.Split(".").First());


    private static AffectedTranscriptResource[] GetAffectedTranscripts(AffectedTranscriptResource[] values)
    {
        var filtered = values?.Where(value => value.Consequences != null);
        return filtered?.Any() == true ? filtered.ToArray() : null;
    }
}
