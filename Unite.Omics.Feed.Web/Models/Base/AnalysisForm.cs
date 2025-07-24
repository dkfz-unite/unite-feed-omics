using Microsoft.AspNetCore.Mvc;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;

namespace Unite.Omics.Feed.Web.Models.Base;

public record AnalysisForm<TEntryModel> where TEntryModel : class, new()
{
    public const double DefaultPloidy = 2.0;

    protected string _donorId;
    protected string _specimenId;
    protected SpecimenType? _specimenType;
    protected string _matchedSpecimenId;
    protected SpecimenType? _matchedSpecimenType;
    protected AnalysisType? _analysisType;
    protected DateOnly? _analysisDate;
    protected int? _analysisDay;
    protected string _genome;
    protected double? _purity;
    protected double? _ploidy;
    protected int? _cells;
    protected string _format;
    protected IFormFile _resourcesFile;
    protected IFormFile _entriesFile;


    [FromForm(Name = "donor_id")]
    public string DonorId { get => _donorId?.Trim(); set => _donorId = value; }

    [FromForm(Name = "specimen_id")]
    public string SpecimenId { get => _specimenId?.Trim(); set => _specimenId = value; }

    [FromForm(Name = "specimen_type")]
    public SpecimenType? SpecimenType { get => _specimenType; set => _specimenType = value; }

    [FromForm(Name = "matched_specimen_id")]
    public string MatchedSpecimenId { get => _matchedSpecimenId?.Trim(); set => _matchedSpecimenId = value; }

    [FromForm(Name = "matched_specimen_type")]
    public SpecimenType? MatchedSpecimenType { get => _matchedSpecimenType; set => _matchedSpecimenType = value; }

    [FromForm(Name = "analysis_type")]
    public AnalysisType? AnalysisType { get => _analysisType; set => _analysisType = value; }

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

    [FromForm(Name = "format")]
    public string Format { get => _format?.Trim(); set => _format = value; }

    [FromForm(Name = "resources")]
    public IFormFile ResourcesFile { get => _resourcesFile; set => _resourcesFile = value; }

    [FromForm(Name = "entries")]
    public IFormFile EntriesFile { get => _entriesFile; set => _entriesFile = value; }


    public AnalysisModel<TEntryModel> Convert()
    {
        var analysis = new AnalysisModel<TEntryModel>
        {
            TargetSample = new SampleModel
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
                Cells = Cells
            }
        };

        if (HasMatchedSample())
        {
            analysis.MatchedSample = new SampleModel
            {
                DonorId = DonorId,
                SpecimenId = MatchedSpecimenId,
                SpecimenType = MatchedSpecimenType,
                AnalysisType = AnalysisType,
                Genome = Genome
            };
        }

        return analysis;
    }


    private bool HasMatchedSample()
    {
        return !string.IsNullOrWhiteSpace(MatchedSpecimenId) && MatchedSpecimenType.HasValue;
    }
}
