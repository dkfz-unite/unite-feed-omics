using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;

namespace Unite.Omics.Feed.Web.Models.Base;

// Do not assigne default values to properties!
// Otherwise, empty model for matched samples will be created and fail to be validated.
public record SampleModel: SubmissionModel
{
    public const double DefaultPloidy = 2.0;


    private string _genome;
    protected string _donorId;
    protected string _specimenId;
    protected SpecimenType? _specimenType;
    protected AnalysisType? _assayType;
    protected DateOnly? _assayDate;
    protected int? _assayDay;
    protected double? _purity;
    protected double? _ploidy;
    protected int? _cells;
    protected string _batch;


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
    /// Assay type (WGS, WES, RNASeq, RNASeqSc)
    /// </summary>
    [JsonPropertyName("analysis_type")]
    [BsonElement("AnalysisType")]
    public virtual AnalysisType? AssayType { get => _assayType; set => _assayType = value; }

    /// <summary>
    /// Assay date
    /// </summary>
    [JsonPropertyName("analysis_date")]
    [BsonElement("AnalysisDate")]
    public virtual DateOnly? AssayDate { get => _assayDate; set => _assayDate = value; }

    /// <summary>
    /// Assay day - relative day since the diagnosis statement when the assay was performed
    /// </summary>
    [JsonPropertyName("analysis_day")]
    [BsonElement("AnalysisDay")]
    public virtual int? AssayDay { get => _assayDay; set => _assayDay = value; }

    /// <summary>
    /// Genome version (GRCh37 or GRCh38)
    /// </summary>
    [JsonPropertyName("genome")]
    public virtual string Genome { get => _genome?.Trim(); set => _genome = value; }

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
    /// Sample cells number (if it's single cell sequencing)
    /// </summary>
    [JsonPropertyName("cells")]
    public virtual int? Cells { get => _cells; set => _cells = value; }

    /// <summary>
    /// Batch information
    /// </summary>
    [JsonPropertyName("batch")]
    public virtual string Batch { get => _batch?.Trim(); set => _batch = value; }

    /// <summary>
    /// Sample resources
    /// </summary>
    [JsonPropertyName("resources")]
    public override ResourceModel[] Resources { get; set; }
}
