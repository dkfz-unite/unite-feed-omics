using System.Text.Json.Serialization;

namespace Unite.Genome.Feed.Web.Models.Base;

public record ResourceModel
{
    private string _type;
    private string _format;
    private string _url;

    /// <summary>
    /// Resource type (Alignment, MEX, etc.)
    /// </summary>
    [JsonPropertyName("type")]
    public virtual string Type { get => _type?.Trim(); set => _type = value; }

    /// <summary>
    /// Resource format (BAM, MEX, etc.)
    /// </summary>
    [JsonPropertyName("format")]
    public virtual string Format { get => _format?.Trim(); set => _format = value; }

    /// <summary>
    /// Resource URL
    /// </summary>
    [JsonPropertyName("url")]
    public virtual string Url { get => _url?.Trim(); set => _url = value; }
}
