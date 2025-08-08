using Microsoft.AspNetCore.Mvc;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Omics.Feed.Web.Models.Base.Binders;

namespace Unite.Omics.Feed.Web.Models.Base;

public record SampleForm
{
    protected string _donorId;
    protected string _specimenId;
    protected string _specimenType;
    protected string _analysisType;
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
    public string SpecimenType { get => _specimenType?.Trim(); set => _specimenType = value; }

    /// <summary>
    /// Analysis type (WGS, WES, RNASeq, RNASeqSc)
    /// </summary>
    [FromForm(Name = "analysis_type")]
    public string AnalysisType { get => _analysisType?.Trim(); set => _analysisType = value; }

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
            SpecimenType = EnumBinder.Bind<SpecimenType>(SpecimenType).Value,
            AnalysisType = EnumBinder.Bind<AnalysisType>(AnalysisType).Value,
            AnalysisDate = AnalysisDate,
            AnalysisDay = AnalysisDay,
            Genome = Genome
        };
    }
}
