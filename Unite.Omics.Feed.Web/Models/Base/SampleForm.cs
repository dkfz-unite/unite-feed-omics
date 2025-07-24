using Microsoft.AspNetCore.Mvc;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Essentials.Tsv;

namespace Unite.Omics.Feed.Web.Models.Base;

public record SampleForm
{
    public const double DefaultPloidy = 2.0;

    protected string _donorId;
    protected string _specimenId;
    protected SpecimenType? _specimenType;
    protected AnalysisType? _analysisType;
    protected DateOnly? _analysisDate;
    protected int? _analysisDay;
    protected string _genome;
    protected double? _purity;
    protected double? _ploidy;
    protected int? _cells;
    protected IFormFile _resourcesFile;
    protected ResourceModel[] _resources;


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
    /// Sample reference genome (e.g. grch37)
    /// </summary>
    [FromForm(Name = "genome")]
    public string Genome { get => _genome?.Trim().ToLower(); set => _genome = value; }

    /// <summary>
    /// Sample purity (TCC)
    /// </summary>
    [FromForm(Name = "purity")]
    public double? Purity { get => _purity; set => _purity = value; }

    /// <summary>
    /// Sample ploidy
    /// </summary>
    [FromForm(Name = "ploidy")]
    public double? Ploidy { get => _ploidy ?? DefaultPloidy; set => _ploidy = value; }

    /// <summary>
    /// Sample cells number (if it's single cell sequencing)
    /// </summary>
    [FromForm(Name = "cells")]
    public int? Cells { get => _cells; set => _cells = value; }

    /// <summary>
    /// Tsv file with resources metadata.
    /// </summary>
    [FromForm(Name = "resources")]
    public IFormFile ResourcesFile { get => _resourcesFile; init => _resourcesFile = value; }

    
    // public ResourceModel[] Resources { get => _resources ?? ParseResources(); }


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
            Genome = Genome,
            Purity = Purity,
            Ploidy = Ploidy,
            Cells = Cells,
            // Resources = Resources
        };
    }


    // private ResourceModel[] ParseResources()
    // {
    //     try
    //     {
    //         using var reader = new StreamReader(ResourcesFile.OpenReadStream());

    //         var tsv = reader.ReadToEnd();

    //         _resources = TsvReader.Read<ResourceModel>(tsv).ToArray();

    //         return _resources;
    //     }
    //     catch
    //     {
    //         return null;
    //     }
    // }
}
