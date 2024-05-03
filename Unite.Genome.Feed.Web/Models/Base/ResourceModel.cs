using System.Text.Json.Serialization;

namespace Unite.Genome.Feed.Web.Models.Base;

public record ResourceModel
{
    private string _type;
    private string _path;
    private string _url;

    /// <summary>
    /// Resource type
    /// </summary>
    [JsonPropertyName("type")]
    public virtual string Type { get => _type?.Trim(); set => _type = value; }

    /// <summary>
    /// Resource path
    /// </summary>
    [JsonPropertyName("path")]
    public virtual string Path { get => _path?.Trim(); set => _path = value; }

    /// <summary>
    /// Resource URL
    /// </summary>
    [JsonPropertyName("url")]
    public virtual string Url { get => _url?.Trim(); set => _url = value; }


    public virtual bool IsNotEmpty()
    {
        return !string.IsNullOrWhiteSpace(Type)
            || !string.IsNullOrWhiteSpace(Path)
            || !string.IsNullOrWhiteSpace(Url);
    }
}
