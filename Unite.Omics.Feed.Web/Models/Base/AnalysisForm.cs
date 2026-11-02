using Microsoft.AspNetCore.Mvc;

namespace Unite.Omics.Feed.Web.Models.Base;

public record AnalysisForm : SampleForm
{
    public const double DefaultPloidy = 2.0;


    private string _entriesScope;
    protected string _matchedSpecimenId;
    protected string _matchedSpecimenType;
    protected double? _purity;
    protected double? _ploidy;
    protected int? _cells;


    [FromForm(Name = "matched_specimen_id")]
    public string MatchedSpecimenId { get => _matchedSpecimenId?.Trim(); set => _matchedSpecimenId = value; }

    [FromForm(Name = "matched_specimen_type")]
    public string MatchedSpecimenType { get => _matchedSpecimenType?.Trim(); set => _matchedSpecimenType = value; }

    [FromForm(Name = "entries_scope")]
    public string EntriesScope { get => _entriesScope?.Trim(); set => _entriesScope = value; }

    [FromForm(Name = "purity")]
    public double? Purity { get => _purity; set => _purity = value; }

    [FromForm(Name = "ploidy")]
    public double? Ploidy { get => _ploidy ?? DefaultPloidy; set => _ploidy = value; }

    [FromForm(Name = "cells")]
    public int? Cells { get => _cells; set => _cells = value; }
}
