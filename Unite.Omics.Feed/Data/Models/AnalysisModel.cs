namespace Unite.Omics.Feed.Data.Models;

public record AnalysisModel
{
    public SampleModel TargetSample { get; set; }
    public SampleModel MatchedSample { get; set; }
    public IEnumerable<ResourceModel> Resources { get; set; }
}

public record AnalysisModel<TEntry> : AnalysisModel
{
    public TEntry[] Entries { get; set; }
}
