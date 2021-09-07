using System.Text.Json.Serialization;
using Unite.Data.Entities.Mutations.Enums;
using Unite.Data.Extensions;

namespace Unite.Mutations.Annotations.Clients.Vep.Resources
{
    internal class MutationResource
    {
        [JsonPropertyName("input")]
        public string Input { get; set; }

        [JsonPropertyName("seq_region_name")]
        [JsonConverter(typeof(JsonStringEnumMemberConverter))]
        public Chromosome Chromosome { get; set; }

        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("end")]
        public int End { get; set; }

        [JsonPropertyName("allele_string")]
        public string AlleleChange { get; set; }

        [JsonIgnore]
        public string ReferenceBase => AlleleChange.Split("/")[0];

        [JsonIgnore]
        public string AlternateBase => AlleleChange.Split("/")[1];

        [JsonIgnore]
        public int Position => ReferenceBase == "-" ? End : Start;

        [JsonIgnore]
        public string Code => $"chr{Chromosome.ToDefinitionString()}:g.{Position}{ReferenceBase}>{AlternateBase}";


        [JsonPropertyName("transcript_consequences")]
        public AffectedTranscriptResource[] AffectedTranscripts { get; set; }
    }
}
