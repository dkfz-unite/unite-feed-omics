using System.Text.Json.Serialization;

namespace Unite.Omics.Feed.Web.Models.Base;

public record AnalysisModel
{
    protected ResourceModel[] _resources;

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
}

public record AnalysisModel<TEntryModel> : AnalysisModel where TEntryModel : class, new()
{
    protected TEntryModel[] _entries;

    /// <summary>
    /// Entries data.
    /// </summary>
    [JsonPropertyName("entries")]
    public virtual TEntryModel[] Entries { get => _entries?.Distinct().ToArray(); set => _entries = value; }
}
