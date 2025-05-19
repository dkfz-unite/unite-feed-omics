using System.Text.Json.Serialization;

namespace Unite.Omics.Annotations.Clients.Uniprot.Resources;

public class ProteinDomainFragment
{
    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("end")]
    public int End { get; set; }
}
