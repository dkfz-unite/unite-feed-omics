using System.Text.Json.Serialization;
using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Variants.SV.Enums;

namespace Unite.Genome.Feed.Web.Models.Variants.SV;

public class VariantModel
{
    private Chromosome? _chromosome;
    private int? _start;
    private int? _end;
    private Chromosome? _otherChromosome;
    private int? _otherStart;
    private int? _otherEnd;
    private SvType? _type;
    private bool? _inverted;
    private string _flankingSequenceFrom;
    private string _flankingSequenceTo;


    /// <summary>
    /// Frist breakpoint chromosome.
    /// </summary>
    [JsonPropertyName("chromosome_1")]
    public Chromosome? Chromosome { get => _chromosome; set => _chromosome = value; }

    /// <summary>
    /// Frist breakpoint start..
    /// </summary>
    [JsonPropertyName("start_1")]
    public int? Start { get => _start; set => _start = value; }

    /// <summary>
    /// Frist breakpoint end.
    /// </summary>
    [JsonPropertyName("end_1")]
    public int? End { get => _end ?? _start + 1; set => _end = value; }

    /// <summary>
    /// Flanking genomic sequence 200bp around first breakpoint.
    /// </summary>
    [JsonPropertyName("flanking_sequence_1")]
    public string FlankingSequenceFrom { get => _flankingSequenceFrom; set => _flankingSequenceFrom = value; }

    /// <summary>
    /// Second breakpoint chromosome.
    /// </summary>
    [JsonPropertyName("chromosome_2")]
    public Chromosome? OtherChromosome { get => _otherChromosome; set => _otherChromosome = value; }

    /// <summary>
    /// Second breakpoint start.
    /// </summary>
    [JsonPropertyName("start_2")]
    public int? OtherStart { get => _otherStart; set => _otherStart = value; }

    /// <summary>
    /// Second breakpoint end.
    /// </summary>
    [JsonPropertyName("end_2")]
    public int? OtherEnd { get => _otherEnd ?? _otherStart + 1; set => _otherEnd = value; }

    /// <summary>
    /// Flanking genomic sequence 200bp around second breakpoint.
    /// </summary>
    [JsonPropertyName("flanking_sequence_2")]
    public string FlankingSequenceTo { get => _flankingSequenceTo; set => _flankingSequenceTo = value; }

    /// <summary>
    /// Structural variant type.
    /// </summary>
    [JsonPropertyName("type")]
    public SvType? Type { get => _type; set => _type = value; }

    /// <summary>
    /// Whether event is inverted or not.
    /// </summary>
    [JsonPropertyName("inverted")]
    public bool? Inverted { get => _inverted; set => _inverted = value; }
}
