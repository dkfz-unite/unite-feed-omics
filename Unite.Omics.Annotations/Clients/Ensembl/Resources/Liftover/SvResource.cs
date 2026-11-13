using System.Text.Json.Serialization;
using Unite.Data.Entities.Omics.Analysis.Dna.Sv.Enums;
using Unite.Data.Entities.Omics.Enums;

namespace Unite.Omics.Annotations.Clients.Ensembl.Resources.Liftover;

public class SvResource
{
    [JsonPropertyName("chromosome_1")]
    public Chromosome? Chromosome { get; set; }

    [JsonPropertyName("start_1")]
    public int? Start { get; set; }

    [JsonPropertyName("end_1")]
    public int? End { get; set; }

    [JsonPropertyName("chromosome_2")]
    public Chromosome? OtherChromosome { get; set; }

    [JsonPropertyName("start_2")]
    public int? OtherStart { get; set; }

    [JsonPropertyName("end_2")]
    public int? OtherEnd { get; set; }

    [JsonPropertyName("type")]
    public SvType? Type { get; set; }

    [JsonPropertyName("inverted")]
    public bool? Inverted { get; set; }
}
