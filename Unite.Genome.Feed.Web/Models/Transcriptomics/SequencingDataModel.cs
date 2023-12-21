using System.Text.Json.Serialization;

namespace Unite.Genome.Feed.Web.Models.Transcriptomics;

public record SequencingDataModel<TEntry> : Base.SequencingDataModel<TEntry>
    where TEntry : class, new()
{
    [JsonPropertyName("expressions")]
    public override TEntry[] Entries { get => base.Entries; set => base.Entries = value; }
}
