using System.Text.Json.Serialization;

namespace Unite.Genome.Feed.Web.Models.Base;

public record SequencingDataModel<TEntryModel>
    where TEntryModel : class, new()
{
    protected ResourceModel[] _resources;
    protected TEntryModel[] _entries;

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
    /// Resources data.
    /// </summary>
    [JsonPropertyName("resources")]
    public virtual ResourceModel[] Resources { get => _resources?.Distinct().ToArray(); set => _resources = value; }

    /// <summary>
    /// Entries data.
    /// </summary>
    [JsonPropertyName("entries")]
    public virtual TEntryModel[] Entries { get => _entries?.Distinct().ToArray(); set => _entries = value; }
}
