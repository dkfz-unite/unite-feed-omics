using System.Text.Json.Serialization;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Annotations.Vep.Client.Resources
{
    internal class AffectedTranscriptResource
    {
        [JsonPropertyName("consequence_terms")]
        public ConsequenceType[] Consequences { get; set; }


        [JsonPropertyName("gene_id")]
        public string GeneId { get; set; }

        [JsonPropertyName("gene_symbol")]
        public string GeneSymbol { get; set; }


        [JsonPropertyName("transcript_id")]
        public string TranscriptId { get; set; }

        [JsonPropertyName("biotype")]
        public string TranscriptBiotype { get; set; }


        [JsonPropertyName("strand")]
        public int? Strand { get; set; }


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
    }
}
