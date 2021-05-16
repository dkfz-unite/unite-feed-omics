using System.Text.Json.Serialization;
using Unite.Data.Entities.Mutations.Enums;

namespace Unite.Mutations.Annotations.Vep.Client.Resources
{
    internal class AnnotationsResource
    {
        [JsonPropertyName("input")]
        public string Code { get; set; }

        [JsonPropertyName("seq_region_name")]
        public Chromosome Chromosome { get; set; }

        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("end")]
        public int End { get; set; }

        [JsonPropertyName("allele_string")]
        public string AlleleChange { get; set; }


        [JsonPropertyName("transcript_consequences")]
        public AffectedTranscriptResource[] AffectedTranscripts { get; set; }
    }
}
