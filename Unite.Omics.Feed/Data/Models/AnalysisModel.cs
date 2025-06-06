using Unite.Data.Entities.Omics.Analysis.Enums;

namespace Unite.Omics.Feed.Data.Models;

public class AnalysisModel
{
    public AnalysisType Type;
    public DateOnly? Date;
    public int? Day;
    public Dictionary<string, string> Parameters;
}
