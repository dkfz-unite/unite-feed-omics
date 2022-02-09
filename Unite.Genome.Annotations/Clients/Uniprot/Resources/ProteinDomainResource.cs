using System.Text.Json.Serialization;

namespace Unite.Genome.Annotations.Clients.Uniprot.Resources
{
    public class ProteinDomainResource
    {
        [JsonPropertyName("accession")]
        public string Id { get; set; }

        [JsonPropertyName("entry_protein_locations")]
        public ProteinDomainLocation[] Locations { get; set; }
    }
}
