using Microsoft.AspNetCore.Mvc;

namespace Unite.Omics.Feed.Web.Models.Base;

public record SampleForm: SubmissionForm
{
    protected string _donorId;
    protected string _specimenId;
    protected string _specimenType;
    protected string _assayType;
    protected DateOnly? _assayDate;
    protected int? _assayDay;
    protected string _genome;
    protected string _batch;


    /// <summary>
    /// Sample donor identifier
    /// </summary>
    /// //TODO: move to base class
    [FromForm(Name = "donor_id")]
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    /// <summary>
    /// Specimen identifier
    /// </summary>
    [FromForm(Name = "specimen_id")]
    public string SpecimenId { get => _specimenId?.Trim(); set => _specimenId = value; }

    /// <summary>
    /// Specimen type
    /// </summary>
    [FromForm(Name = "specimen_type")]
    public string SpecimenType { get => _specimenType?.Trim(); set => _specimenType = value; }

    /// <summary>
    /// Assay type (WGS, WES, RNASeq, RNASeqSc)
    /// </summary>
    [FromForm(Name = "analysis_type")]
    public string AssayType { get => _assayType?.Trim(); set => _assayType = value; }

    /// <summary>
    /// Assay date
    /// </summary>
    [FromForm(Name = "analysis_date")]
    public DateOnly? AssayDate { get => _assayDate; set => _assayDate = value; }

    /// <summary>
    /// Assay day - relative day since the diagnosis statement when the assay was performed
    /// </summary>
    [FromForm(Name = "analysis_day")]
    public int? AssayDay { get => _assayDay; set => _assayDay = value; }

    /// <summary>
    /// Genome version (GRCh37 or GRCh38)
    /// </summary>
    [FromForm(Name = "genome")]
    public string Genome { get => _genome?.Trim(); set => _genome = value; }

    /// <summary>
    /// Batch information
    /// </summary>
    [FromForm(Name = "batch")]
    public string Batch { get => _batch?.Trim(); set => _batch = value; }
}
