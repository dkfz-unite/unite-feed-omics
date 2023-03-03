using System.Text.Json.Serialization;
using Unite.Data.Entities.Genome.Enums;

namespace Unite.Genome.Annotations.Clients.Ensembl.Resources;

public record GeneResource : LookupResource
{
    //[JsonPropertyName("seq_region_name")]
    [JsonPropertyName("chromosome")]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public Chromosome Chromosome { get; set; }

    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("end")]
    public int End { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("exonicLength")]
    public int? ExonicLength { get; set; }

    [JsonPropertyName("strand")]
    public bool Strand { get; set; }

    [JsonPropertyName("biotype")]
    public string Biotype { get; set; }

    //[JsonPropertyName("display_name")]
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }


    [JsonPropertyName("transcript")]
    public TranscriptResource Transcript { get; set; }
}
