using System.Text.Json.Serialization;

namespace Unite.Omics.Feed.Web.Models.Dna;

public record LiftoverResult<TVariant> where TVariant : class, new()
{
    [JsonPropertyName("variant")]
    public TVariant Variant { get; set; }

    [JsonPropertyName("failure")]
    public string Failure { get; set; }


    public LiftoverResult()
    {
    }


    public LiftoverResult(TVariant variant, string failure)
    {
        Variant = variant;
        Failure = failure;
    }
}
