using System.Text.Json.Serialization;

namespace Unite.Genome.Annotations.Clients.Uniprot.Resources;

public class ProteinDomainLocation
{
    [JsonPropertyName("fragments")]
    public ProteinDomainFragment[] Fragments { get; set; }
}
