using Unite.Data.Entities.Genome.Analysis.Enums;

namespace Unite.Genome.Feed.Data.Models;

public class AnalysisModel
{
    public AnalysisType Type;
    public DateOnly? Date;
    public int? Day;
    public Dictionary<string, string> Parameters;
}
