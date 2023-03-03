namespace Unite.Genome.Feed.Web.Models.Transcriptomics;

public class TranscriptomicsDataModel
{
    /// <summary>
    /// Analysis data
    /// </summary>
    public AnalysisModel Analysis { get; set; }

    /// <summary>
    /// Sample data
    /// </summary>
    public AnalysedSampleModel Sample { get; set; }

    /// <summary>
    /// Expression data
    /// </summary>
    public ExpressionModel[] Expressions { get; set; }
}
