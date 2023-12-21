using System.Text.Json.Serialization;
using Unite.Data.Entities.Specimens.Enums;

namespace Unite.Genome.Feed.Web.Models.Base;

public record SampleModel
{
    protected string _donorId;
    protected string _specimenId;
    protected SpecimenType? _specimenType;
    protected double? _purity;
    protected double? _ploidy;


    /// <summary>
    /// Sample donor identifier
    /// </summary>
    [JsonPropertyName("donor_id")]
    public virtual string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    /// <summary>
    /// Specimen identifier
    /// </summary>
    [JsonPropertyName("specimen_id")]
    public virtual string SpecimenId { get => _specimenId?.Trim(); set => _specimenId = value; }

    /// <summary>
    /// Specimen type
    /// </summary>
    [JsonPropertyName("specimen_type")]
    public virtual SpecimenType? SpecimenType { get => _specimenType; set => _specimenType = value; }

    /// <summary>
    /// Sample purity (TCC)
    /// </summary>
    [JsonPropertyName("purity")]
    public virtual double? Purity { get => _purity; set => _purity = value; }

    /// <summary>
    /// Sample ploidy
    /// </summary>
    [JsonPropertyName("ploidy")]
    public virtual double? Ploidy { get => _ploidy; set => _ploidy = value; }

    /// <summary>
    /// Default sample ploidy
    /// </summary>
    public virtual double DefaultPloidy => 2;
}
