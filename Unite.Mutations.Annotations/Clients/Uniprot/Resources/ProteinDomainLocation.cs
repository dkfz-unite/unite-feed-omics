using System.Text.Json.Serialization;

namespace Unite.Mutations.Annotations.Clients.Uniprot.Resources
{
    public class ProteinDomainLocation
    {
        [JsonPropertyName("fragments")]
        public ProteinDomainFragment[] Fragments { get; set; }
    }
}
