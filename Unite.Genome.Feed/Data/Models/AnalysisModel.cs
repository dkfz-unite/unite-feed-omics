using Unite.Data.Entities.Genome.Analysis.Enums;

namespace Unite.Genome.Feed.Data.Models;

public class AnalysisModel
{
    public string ReferenceId { get; set; }
    public AnalysisType? Type { get; set; }
    public DateOnly? Date { get; set; }
    public Dictionary<string, string> Parameters { get; set; }

    public IEnumerable<AnalysedSampleModel> AnalysedSamples { get; set; }
}
