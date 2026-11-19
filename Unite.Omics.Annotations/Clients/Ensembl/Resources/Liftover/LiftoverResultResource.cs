using System.Text.Json.Serialization;

namespace Unite.Omics.Annotations.Clients.Ensembl.Resources.Liftover;

public class LiftoverResultResource<TVariant>
{
    [JsonPropertyName("source")]
    public TVariant Source { get; set; }

    [JsonPropertyName("target")]
    public TVariant Target { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; }
}
