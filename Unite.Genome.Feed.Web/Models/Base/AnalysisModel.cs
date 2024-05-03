
using System.Text.Json.Serialization;
using Unite.Data.Entities.Genome.Analysis.Enums;
using Unite.Essentials.Extensions;

namespace Unite.Genome.Feed.Web.Models.Base;

public record AnalysisModel
{
    protected string _id;
    protected AnalysisType? _type;
    protected DateOnly? _date;
    protected int? _day;

    protected Dictionary<string, string> _parameters;


    /// <summary>
    /// Analysis identifier
    /// </summary>
    [JsonPropertyName("id")]
    public virtual string Id { get => _id?.Trim(); set => _id = value; }

    /// <summary>
    /// Type of the analysis (WGS, WES)
    /// </summary>
    [JsonPropertyName("type")]
    public virtual AnalysisType? Type { get => _type; set => _type = value; }

    /// <summary>
    /// Date when the analysis was performed
    /// </summary>
    [JsonPropertyName("date")]
    public virtual DateOnly? Date { get => _date; set => _date = value; }

    /// <summary>
    /// Relative day since the diagnisis statement when the analysis was performed
    /// </summary>
    [JsonPropertyName("day")]
    public virtual int? Day { get => _day; set => _day = value; }  
     
    /// <summary>
    /// Analysis parameters
    /// </summary>
    [JsonPropertyName("parameters")]
    public virtual Dictionary<string, string> Parameters { get => Trim(_parameters); set => _parameters = value; }


    public virtual bool IsNotEmpty()
    {
        return !string.IsNullOrWhiteSpace(Id)
            || Type.HasValue
            || Date.HasValue
            || Day.HasValue
            || Parameters.IsNotEmpty();
    
    }


    protected static Dictionary<string, string> Trim(Dictionary<string, string> dictionary)
    {
        return dictionary?.ToDictionary(entry => entry.Key.Trim(), entry => entry.Value.Trim());
    }
}
