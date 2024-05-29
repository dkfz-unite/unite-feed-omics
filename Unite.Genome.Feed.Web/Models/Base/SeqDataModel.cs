using System.Text.Json.Serialization;

namespace Unite.Genome.Feed.Web.Models.Base;

public record SeqDataModel<TEntryModel>
    where TEntryModel : class, new()
{
    protected TEntryModel[] _entries;

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
    public virtual TEntryModel[] Entries { get => _entries?.Distinct().ToArray(); set => _entries = value; }
}
