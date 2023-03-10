using Unite.Genome.Feed.Data.Models.Enums;

namespace Unite.Genome.Feed.Web.Models.Base;

public class SampleModel
{
    private string _id;
    private string _donorId;
    private string _specimenId;
    private SpecimenType? _specimenType;
    private double? _ploidy;
    private double? _purity;


    /// <summary>
    /// Sample identifier
    /// </summary>
    public string Id { get => _id?.Trim(); set => _id = value; }

    /// <summary>
    /// Sample donor identifier
    /// </summary>
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    /// <summary>
    /// Specimen identifier
    /// </summary>
    public string SpecimenId { get => _specimenId?.Trim(); set => _specimenId = value; }

    /// <summary>
    /// Specimen type
    /// </summary>
    public SpecimenType? SpecimenType { get => _specimenType; set => _specimenType = value; }

    /// <summary>
    /// Sample ploidy
    /// </summary>
    public double? Ploidy { get => _ploidy; set => _ploidy = value; }

    /// <summary>
    /// Sample purity (TCC)
    /// </summary>
    public double? Purity { get => _purity; set => _purity = value; }

    /// <summary>
    /// Default sample ploidy
    /// </summary>
    public double DefaultPloidy => 2;
}
