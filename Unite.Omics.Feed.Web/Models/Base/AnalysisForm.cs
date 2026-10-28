using Microsoft.AspNetCore.Mvc;

namespace Unite.Omics.Feed.Web.Models.Base;

public record AnalysisForm : SampleForm
{
    public const double DefaultPloidy = 2.0;


    protected string _matchedSpecimenId;
    protected string _matchedSpecimenType;
    private string[] _scope;
    protected double? _purity;
    protected double? _ploidy;
    protected int? _cells;


    [FromForm(Name = "matched_specimen_id")]
    public string MatchedSpecimenId { get => _matchedSpecimenId?.Trim(); set => _matchedSpecimenId = value; }

    [FromForm(Name = "matched_specimen_type")]
    public string MatchedSpecimenType { get => _matchedSpecimenType?.Trim(); set => _matchedSpecimenType = value; }

    [FromForm(Name = "scope")]
    public string[] Scope { get => _scope?.Select(type => type?.Trim()).ToArray(); set => _scope = value; }

    [FromForm(Name = "purity")]
    public double? Purity { get => _purity; set => _purity = value; }

    [FromForm(Name = "ploidy")]
    public double? Ploidy { get => _ploidy ?? DefaultPloidy; set => _ploidy = value; }

    [FromForm(Name = "cells")]
    public int? Cells { get => _cells; set => _cells = value; }
}
