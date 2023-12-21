using System.Text.Json.Serialization;

namespace Unite.Genome.Feed.Web.Models.Variants;

public record SequencingDataModel<TEntry> : Base.SequencingDataModel<TEntry>
    where TEntry : class, new()
{
    [JsonPropertyName("variants")]
    public override TEntry[] Entries { get => base.Entries; set => base.Entries = value; }
}
