using System.Text.Json.Serialization;
using Unite.Essentials.Tsv.Attributes;

namespace Unite.Omics.Feed.Web.Models.Base;

public record ResourceModel
{
    private string _name;
    private string _type;
    private string _format;
    private string _url;


    /// <summary>
    /// Resource name(case sensitive).
    /// </summary>
    [JsonPropertyName("name")]
    [Column("name")]
    public virtual string Name { get => _name?.Trim().ToLower(); set => _name = value; }

    /// <summary>
    /// Resource type (dna, dna-sm, dna-cnv, dna-sv, meth, meth-lvl, rna, rna-exp, rnasc, rnasc-exp,  etc.).
    /// </summary>
    [JsonPropertyName("type")]
    [Column("type")]
    public virtual string Type { get => _type?.Trim().ToLower(); set => _type = value; }

    /// <summary>
    /// Resource format (bam, idat, vcf, tsv, csv, mex, etc.).
    /// </summary>
    [JsonPropertyName("format")]
    [Column("format")]
    public virtual string Format { get => _format?.Trim().ToLower(); set => _format = value; }

    /// <summary>
    /// Resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    [Column("url")]
    public virtual string Url { get => _url?.Trim().ToLower(); set => _url = value; }
}
