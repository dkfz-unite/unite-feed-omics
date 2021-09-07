using System.Text.Json.Serialization;

namespace Unite.Mutations.Annotations.Clients.Uniprot.Resources
{
    public class ProteinDomainResource
    {
        [JsonPropertyName("accession")]
        public string Id { get; set; }

        [JsonPropertyName("entry_protein_locations")]
        public ProteinDomainLocation[] Locations { get; set; }
    }
}
