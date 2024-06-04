using System.Text.Json.Serialization;

namespace Unite.Genome.Feed.Web.Models.Base;

public record AnalysisModel<TEntryModel>
    where TEntryModel : class, new()
{
    protected TEntryModel[] _entries;

    /// <summary>
    /// Analysed sample.
    /// </summary>
    /// <value></value>
    [JsonPropertyName("tsample")]
    public virtual SampleModel TargetSample { get; set; }

    /// <summary>
    /// Matched sample.
    /// </summary>
    [JsonPropertyName("msample")]
    public virtual SampleModel MatchedSample { get; set; }

    /// <summary>
    /// Entries data.
    /// </summary>
    [JsonPropertyName("entries")]
    public virtual TEntryModel[] Entries { get => _entries?.Distinct().ToArray(); set => _entries = value; }
}
