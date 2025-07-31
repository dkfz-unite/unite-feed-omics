using Microsoft.AspNetCore.Mvc;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;

namespace Unite.Omics.Feed.Web.Models.Base;

public record SampleForm
{
    protected string _donorId;
    protected string _specimenId;
    protected SpecimenType? _specimenType;
    protected AnalysisType? _analysisType;
    protected DateOnly? _analysisDate;
    protected int? _analysisDay;
    protected string _genome;
    protected IFormFile _resourcesFile;


    /// <summary>
    /// Sample donor identifier
    /// </summary>
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
    public SpecimenType? SpecimenType { get => _specimenType; set => _specimenType = value; }

    /// <summary>
    /// Analysis type (WGS, WES, RNASeq, RNASeqSc)
    /// </summary>
    [FromForm(Name = "analysis_type")]
    public AnalysisType? AnalysisType { get => _analysisType; set => _analysisType = value; }

    /// <summary>
    /// Analysis date
    /// </summary>
    [FromForm(Name = "analysis_date")]
    public DateOnly? AnalysisDate { get => _analysisDate; set => _analysisDate = value; }

    /// <summary>
    /// Analysis day - relative day since the diagnosis statement when the analysis was performed
    /// </summary> 
    [FromForm(Name = "analysis_day")]
    public int? AnalysisDay { get => _analysisDay; set => _analysisDay = value; }

    /// <summary>
    /// Genome version (GRCh37 or GRCh38)
    /// </summary>
    [FromForm(Name = "genome")]
    public string Genome { get => _genome?.Trim(); set => _genome = value; }

    /// <summary>
    /// Tsv file with resources metadata.
    /// </summary>
    [FromForm(Name = "resources")]
    public IFormFile ResourcesFile { get => _resourcesFile; init => _resourcesFile = value; }


    public SampleModel Convert()
    {
        return new SampleModel
        {
            DonorId = DonorId,
            SpecimenId = SpecimenId,
            SpecimenType = SpecimenType,
            AnalysisType = AnalysisType,
            AnalysisDate = AnalysisDate,
            AnalysisDay = AnalysisDay,
            Genome = Genome
        };
    }
}
