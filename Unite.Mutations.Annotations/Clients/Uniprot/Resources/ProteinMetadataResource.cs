using System.Text.Json.Serialization;

namespace Unite.Mutations.Annotations.Clients.Uniprot.Resources
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
