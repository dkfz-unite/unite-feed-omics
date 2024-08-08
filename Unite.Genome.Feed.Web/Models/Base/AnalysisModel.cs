using System.Text.Json.Serialization;

namespace Unite.Genome.Feed.Web.Models.Base;

public record AnalysisModel<TEntryModel>
    where TEntryModel : class, new()
{
    protected ResourceModel[] _resources;
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
