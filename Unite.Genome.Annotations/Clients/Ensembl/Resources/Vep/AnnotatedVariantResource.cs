using System.Text.Json.Serialization;

namespace Unite.Genome.Annotations.Clients.Ensembl.Resources.Vep;

internal class AnnotatedVariantResource
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("input")]
    public string Input { get; set; }

    //[JsonPropertyName("seq_region_name")]
    //[JsonConverter(typeof(JsonStringEnumMemberConverter))]
    //public Chromosome Chromosome { get; set; }

    //[JsonPropertyName("start")]
    //public int Start { get; set; }

    //[JsonPropertyName("end")]
    //public int End { get; set; }

    //[JsonPropertyName("allele_string")]
    //public string AlleleChange { get; set; }

    //[JsonIgnore]
    //public string ReferenceBase => AlleleChange.Split("/")[0];

    //[JsonIgnore]
    //public string AlternateBase => AlleleChange.Split("/")[1];

    //[JsonIgnore]
    //public int Position => ReferenceBase == "-" ? End : Start;

    //[JsonIgnore]
    //public string Code => $"chr{Chromosome.ToDefinitionString()}:g.{Position}{ReferenceBase}>{AlternateBase}";


    [JsonPropertyName("transcript_consequences")]
    public AffectedTranscriptResource[] AffectedTranscripts { get; set; }

    [JsonIgnore]
    public long VariantId => long.Parse(Id.Split(".").First());
}
