using System.Text.Json.Serialization;

namespace Unite.Genome.Feed.Web.Models.Base;

public record SequencingDataModel<TEntry>
    where TEntry : class, new()
{
    protected TEntry[] _entries;

    /// <summary>
    /// Analysis data.
    /// </summary>
    [JsonPropertyName("analysis")]
    public virtual AnalysisModel Analysis { get; set; }

    /// <summary>
    /// Analysed sample.
    /// </summary>
    /// <value></value>
    [JsonPropertyName("target_sample")]
    public virtual SampleModel TargetSample { get; set; }

    /// <summary>
    /// Matched sample.
    /// </summary>
    [JsonPropertyName("matched_sample")]
    public virtual SampleModel MatchedSample { get; set; }

    /// <summary>
    /// Entries data.
    /// </summary>
    [JsonPropertyName("entries")]
    public virtual TEntry[] Entries { get => _entries?.Distinct().ToArray(); set => _entries = value; }
}
