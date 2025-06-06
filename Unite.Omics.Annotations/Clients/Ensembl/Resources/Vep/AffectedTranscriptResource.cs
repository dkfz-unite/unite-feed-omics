using System.Text.Json.Serialization;

namespace Unite.Omics.Annotations.Clients.Ensembl.Resources.Vep;

internal record AffectedTranscriptResource
{
    [JsonPropertyName("gene_id")]
    public string GeneId { get; set; }

    [JsonPropertyName("transcript_id")]
    public string TranscriptId { get; set; }

    [JsonPropertyName("canonical")]
    public int? Canonical { get; set; }


    [JsonPropertyName("cdna_start")]
    public int? CDNAStart { get; set; }

    [JsonPropertyName("cdna_end")]
    public int? CDNAEnd { get; set; }

    [JsonPropertyName("cds_start")]
    public int? CDSStart { get; set; }

    [JsonPropertyName("cds_end")]
    public int? CDSEnd { get; set; }

    [JsonPropertyName("protein_start")]
    public int? ProteinStart { get; set; }

    [JsonPropertyName("protein_end")]
    public int? ProteinEnd { get; set; }

    [JsonPropertyName("amino_acids")]
    public string ProteinChange { get; set; }

    [JsonPropertyName("codons")]
    public string CodonChange { get; set; }

    [JsonPropertyName("bp_overlap")]
    public int? OverlapBpNumber { get; set; }

    [JsonPropertyName("percentage_overlap")]
    public double? OverlapPercentage { get; set; }

    [JsonPropertyName("distance")]
    public int? Distance { get; set; }

    [JsonPropertyName("consequence_terms")]
    public string[] Effects { get; set; }


    public bool IsCanonical
    {
        get => Canonical == 1;
    }

    public bool IsIntergenic
    {
        get => OverlapPercentage == 100
            && Effects.Length == 1
            && Effects.Contains("intergenic_variant");
    }
}
