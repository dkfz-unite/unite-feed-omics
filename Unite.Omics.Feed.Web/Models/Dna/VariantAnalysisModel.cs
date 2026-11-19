using System.Text.Json.Serialization;

namespace Unite.Omics.Feed.Web.Models.Dna;

public record VariantAnalysisModel<TVariant> : Base.AnalysisModel<TVariant>
    where TVariant : class, new()
{
    [JsonPropertyName("liftover_failures")]
    public LiftoverResult<TVariant>[] LiftoverFailures { get; set; }
}
