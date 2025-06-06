using System.Text.Json.Serialization;

namespace Unite.Omics.Annotations.Clients.Uniprot.Resources;

public class ProteinDomainLocation
{
    [JsonPropertyName("fragments")]
    public ProteinDomainFragment[] Fragments { get; set; }
}
