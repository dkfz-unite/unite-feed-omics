using System.Text.Json.Serialization;
using Unite.Data.Entities.Omics.Enums;
using Unite.Essentials.Tsv.Attributes;

namespace Unite.Omics.Feed.Web.Models.Dna.CnvProfile;

public class CnvProfileModel
{
    [JsonPropertyName("chromosome")]
    [Column("chromosome")]
    public Chromosome Chromosome { get; set; }
    [JsonPropertyName("chromosome_arm")]
    [Column("chromosome_arm")]
    public ChromosomeArm ChromosomeArm { get; set; }
    [JsonPropertyName("gain")]
    [Column("gain")]
    public float Gain { get; set; }
    [JsonPropertyName("loss")]
    [Column("loss")]
    public float Loss { get; set; }
    [JsonPropertyName("neutral")]
    [Column("neutral")]
    public float Neutral { get; set; }
}