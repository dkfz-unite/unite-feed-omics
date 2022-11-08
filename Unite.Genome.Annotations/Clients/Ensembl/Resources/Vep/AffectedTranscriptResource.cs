using System.Text.Json.Serialization;

namespace Unite.Genome.Annotations.Clients.Ensembl.Resources.Vep;

internal class AffectedTranscriptResource
{
    [JsonPropertyName("gene_id")]
    public string GeneId { get; set; }

    [JsonPropertyName("transcript_id")]
    public string TranscriptId { get; set; }


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
    public string AminoAcidChange { get; set; }

    [JsonPropertyName("codons")]
    public string CodonChange { get; set; }

    [JsonPropertyName("bp_overlap")]
    public int? OverlapBpNumber { get; set; }

    [JsonPropertyName("percentage_overlap")]
    public double? OverlapPercentage { get; set; }

    [JsonPropertyName("distance")]
    public int? Distance { get; set; }


    [JsonPropertyName("consequence_terms")]
    public string[] Consequences { get; set; }
}
