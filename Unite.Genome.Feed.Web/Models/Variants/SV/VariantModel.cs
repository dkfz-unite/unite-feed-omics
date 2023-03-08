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
    [JsonPropertyName("Chromosome1")]
    public Chromosome? Chromosome { get => _chromosome; set => _chromosome = value; }

    /// <summary>
    /// Frist breakpoint start..
    /// </summary>
    [JsonPropertyName("Start1")]
    public int? Start { get => _start; set => _start = value; }

    /// <summary>
    /// Frist breakpoint end.
    /// </summary>
    [JsonPropertyName("End1")]
    public int? End { get => _end ?? _start + 1; set => _end = value; }

    /// <summary>
    /// Second breakpoint chromosome.
    /// </summary>
    [JsonPropertyName("Chromosome2")]
    public Chromosome? OtherChromosome { get => _otherChromosome; set => _otherChromosome = value; }

    /// <summary>
    /// Second breakpoint start.
    /// </summary>
    [JsonPropertyName("Start2")]
    public int? OtherStart { get => _otherStart; set => _otherStart = value; }

    /// <summary>
    /// Second breakpoint end.
    /// </summary>
    [JsonPropertyName("End2")]
    public int? OtherEnd { get => _otherEnd ?? _otherStart + 1; set => _otherEnd = value; }

    /// <summary>
    /// Structural variant type.
    /// </summary>
    [JsonPropertyName("Type")]
    public SvType? Type { get => _type; set => _type = value; }

    /// <summary>
    /// Whether event is inverted or not.
    /// </summary>
    [JsonPropertyName("Inverted")]
    public bool? Inverted { get => _inverted; set => _inverted = value; }

    /// <summary>
    /// Flanking genomic sequence 200bp around first breakpoint.
    /// </summary>
    [JsonPropertyName("FlankingSequence1")]
    public string FlankingSequenceFrom { get => _flankingSequenceFrom; set => _flankingSequenceFrom = value; }

    /// <summary>
    /// Flanking genomic sequence 200bp around second breakpoint.
    /// </summary>
    [JsonPropertyName("FlankingSequence2")]
    public string FlankingSequenceTo { get => _flankingSequenceTo; set => _flankingSequenceTo = value; }


    #region Equality
    public override bool Equals(object obj)
    {
        var other = obj as VariantModel;

        if (other == null) return false;

        return Chromosome == other.Chromosome
            && Start == other.Start
            && End == other.End
            && OtherChromosome == other.OtherChromosome
            && OtherStart == other.OtherStart
            && OtherEnd == other.OtherEnd
            && Type == other.Type
            && Inverted == other.Inverted
            && FlankingSequenceFrom == other.FlankingSequenceFrom
            && FlankingSequenceTo == other.FlankingSequenceTo;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 36613;

            hash = hash * 37724 + Chromosome.GetHashCode();
            hash = hash * 37724 + Start.GetHashCode();
            hash = hash * 37724 + End.GetHashCode();
            hash = hash * 37724 + OtherChromosome.GetHashCode();
            hash = hash * 37724 + OtherStart.GetHashCode();
            hash = hash * 37724 + OtherEnd.GetHashCode();
            hash = hash * 37724 + Type.GetHashCode();
            hash = hash * 37724 + Inverted.GetHashCode();
            hash = hash * 37724 + FlankingSequenceFrom.GetHashCode();
            hash = hash * 37724 + FlankingSequenceTo.GetHashCode();

            return hash;
        }
    }
    #endregion
}
