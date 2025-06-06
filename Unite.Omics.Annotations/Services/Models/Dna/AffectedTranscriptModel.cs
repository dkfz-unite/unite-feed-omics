﻿namespace Unite.Omics.Annotations.Services.Models.Dna;

public class AffectedTranscriptModel
{
    public int VariantId;

    public int? CDNAStart;
    public int? CDNAEnd;
    public int? CDSStart;
    public int? CDSEnd;
    public int? ProteinStart;
    public int? ProteinEnd;
    public string AminoAcidChange;
    public string CodonChange;
    public int? OverlapBpNumber;
    public double? OverlapPercentage;
    public int? Distance;
    public string[] Effects;

    public GeneModel Gene;
    public TranscriptModel Transcript;
    public ProteinModel Protein;
}
