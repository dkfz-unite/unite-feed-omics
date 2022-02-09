using System.Text.Json.Serialization;

namespace Unite.Genome.Annotations.Clients.Uniprot.Resources
{
    public class ProteinMetadataResource
    {
        [JsonPropertyName("accession")]
        public string Id { get; set; }

        [JsonPropertyName("id")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }
    }
}
