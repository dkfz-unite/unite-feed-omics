using Microsoft.AspNetCore.Mvc;
using Unite.Data.Entities.Omics.Analysis.Enums;
using Unite.Data.Entities.Specimens.Enums;
using Unite.Omics.Feed.Web.Models.Base.Binders;

namespace Unite.Omics.Feed.Web.Models.Base;

public record AnalysisForm
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
    protected IFormFile _resourcesFile;


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

    [FromForm(Name = "resources")]
    public IFormFile ResourcesFile { get => _resourcesFile; set => _resourcesFile = value; }


    protected virtual SampleModel ConvertSample()
    {
        return new SampleModel
        {
            DonorId = DonorId,
            SpecimenId = SpecimenId,
            SpecimenType = EnumBinder.Bind<SpecimenType>(SpecimenType).Value,
            AnalysisType = EnumBinder.Bind<AnalysisType>(AnalysisType).Value,
            AnalysisDate = AnalysisDate,
            AnalysisDay = AnalysisDay,
            Genome = Genome,
            Purity = Purity,
            Ploidy = Ploidy,
            Cells = Cells
        };
    }

    protected virtual SampleModel ConvertMatchedSample()
    {
        if (!HasMatchedSample())
            return null;

        return new SampleModel
        {
            DonorId = DonorId,
            SpecimenId = MatchedSpecimenId,
            SpecimenType = EnumBinder.Bind<SpecimenType>(MatchedSpecimenType).Value,
            AnalysisType = EnumBinder.Bind<AnalysisType>(AnalysisType).Value,
            Genome = Genome
        };
    }


    public virtual AnalysisModel Convert()
    {
        return new AnalysisModel
        {
            TargetSample = ConvertSample(),
            MatchedSample = ConvertMatchedSample()
        };
    }


    protected bool HasMatchedSample()
    {
        return !string.IsNullOrWhiteSpace(MatchedSpecimenId) &&
               !string.IsNullOrWhiteSpace(MatchedSpecimenType);
    }
}

public record AnalysisForm<TEntryModel> : AnalysisForm where TEntryModel : class, new()
{
    protected IFormFile _entriesFile;


    [FromForm(Name = "entries")]
    public IFormFile EntriesFile { get => _entriesFile; set => _entriesFile = value; }


    public override AnalysisModel<TEntryModel> Convert()
    {
        return new AnalysisModel<TEntryModel>
        {
            TargetSample = ConvertSample(),
            MatchedSample = ConvertMatchedSample()
        };
    }
}
