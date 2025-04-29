using System.Text.Json.Serialization;

namespace Unite.Genome.Feed.Web.Models.Base;

public record ResourceModel
{
    private string _name;
    private string _type;
    private string _format;
    private string _archive;
    private string _url;


    /// <summary>
    /// Resource name(case sensitive).
    /// </summary>
    [JsonPropertyName("name")]
    public virtual string Name {  get => _name?.TrimFormat(); set => _name = value;  }

    /// <summary>
    /// Resource type (dna, dna-sm, dna-cnv, dna-sv, meth, meth-lvl, rna, rna-exp, rnasc, rnasc-exp,  etc.).
    /// </summary>
    [JsonPropertyName("type")]
    public virtual string Type { get => _type?.Trim().ToLower(); set => _type = value; }

    /// <summary>
    /// Resource format (bam, idat, vcf, tsv, csv, mex, etc.).
    /// </summary>
    [JsonPropertyName("format")]
    public virtual string Format { get => _format?.Trim().ToLower(); set => _format = value; }

    /// <summary>
    /// Resource archive (zip, gz, etc.).
    /// </summary>
    [JsonPropertyName("archive")]
    public virtual string Archive { get => _archive?.Trim().ToLower(); set => _archive = value; }

    /// <summary>
    /// Resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public virtual string Url { get => _url?.Trim().ToLower(); set => _url = value; }
}

internal static class NameExtensions
{
    public static string TrimFormat(this string value)
    {
        var index = value.IndexOf('.');
        return index > 0 ? value[..index] : value;
    }
}
