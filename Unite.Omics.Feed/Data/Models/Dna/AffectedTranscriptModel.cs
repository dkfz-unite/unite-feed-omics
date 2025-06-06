﻿namespace Unite.Omics.Feed.Data.Models.Dna;

public class AffectedTranscriptModel
{
    public int VariantId;

    public int? CDNAStart;
    public int? CDNAEnd;
    public int? CDSStart;
    public int? CDSEnd;
    public int? AAStart;
    public int? AAEnd;
    public string ProteinChange;
    public string CodonChange;
    public int? OverlapBpNumber;
    public double? OverlapPercentage;
    public int? Distance;
    public string[] Consequences;

    public GeneModel Gene;
    public TranscriptModel Transcript;
    public ProteinModel Protein;
}
