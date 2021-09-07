using System.Text.Json.Serialization;

namespace Unite.Mutations.Annotations.Clients.Ensembl.Resources
{
    public class ProteinResource : IEnsemblResource
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("end")]
        public int End { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }
    }
}
