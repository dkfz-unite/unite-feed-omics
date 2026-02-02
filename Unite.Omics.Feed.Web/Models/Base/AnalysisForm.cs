using Microsoft.AspNetCore.Mvc;

namespace Unite.Omics.Feed.Web.Models.Base;

public record AnalysisForm: SubmissionForm
{
    public const double DefaultPloidy = 2.0;

    protected string _donorId;
    protected string _specimenId;
    protected string _specimenType;
    protected string _matchedSpecimenId;
    protected string _matchedSpecimenType;
    protected string _analysisType;
    protected DateOnly? _analysisDate;
    protected int? _analysisDay;
    protected string _genome;
    protected double? _purity;
    protected double? _ploidy;
    protected int? _cells;
    protected string _format;
    protected IFormFile _entriesFile;


    [FromForm(Name = "donor_id")]
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    [FromForm(Name = "specimen_id")]
    public string SpecimenId { get => _specimenId?.Trim(); set => _specimenId = value; }

    [FromForm(Name = "specimen_type")]
    public string SpecimenType { get => _specimenType?.Trim(); set => _specimenType = value; }

    [FromForm(Name = "matched_specimen_id")]
    public string MatchedSpecimenId { get => _matchedSpecimenId?.Trim(); set => _matchedSpecimenId = value; }

    [FromForm(Name = "matched_specimen_type")]
    public string MatchedSpecimenType { get => _matchedSpecimenType?.Trim(); set => _matchedSpecimenType = value; }

    [FromForm(Name = "analysis_type")]
    public string AnalysisType { get => _analysisType?.Trim(); set => _analysisType = value; }

    [FromForm(Name = "analysis_date")]
    public DateOnly? AnalysisDate { get => _analysisDate; set => _analysisDate = value; }

    [FromForm(Name = "analysis_day")]
    public int? AnalysisDay { get => _analysisDay; set => _analysisDay = value; }

    [FromForm(Name = "genome")]
    public string Genome { get => _genome?.Trim(); set => _genome = value; }

    [FromForm(Name = "purity")]
    public double? Purity { get => _purity; set => _purity = value; }

    [FromForm(Name = "ploidy")]
    public double? Ploidy { get => _ploidy ?? DefaultPloidy; set => _ploidy = value; }

    [FromForm(Name = "cells")]
    public int? Cells { get => _cells; set => _cells = value; }
    
    [FromForm(Name = "entries")]
    public IFormFile EntriesFile { get => _entriesFile; set => _entriesFile = value; }
}
