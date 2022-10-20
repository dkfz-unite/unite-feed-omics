using Unite.Data.Entities.Genome.Analysis.Enums;

namespace Unite.Genome.Feed.Web.Models.Base;

public class AnalysisModel
{
    private string _id;
    private AnalysisType? _type;
    private DateTime? _date;


    /// <summary>
    /// Analysis identifier
    /// </summary>
    public string Id { get => _id?.Trim(); set => _id = value; }

    /// <summary>
    /// Type of the analysis (WGS, WES)
    /// </summary>
    public AnalysisType? Type { get => _type; set => _type = value; }

    /// <summary>
    /// Date when the analysis was performed
    /// </summary>
    public DateTime? Date { get => _date; set => _date = value; }

    /// <summary>
    /// Analysis parameters
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; }
}
