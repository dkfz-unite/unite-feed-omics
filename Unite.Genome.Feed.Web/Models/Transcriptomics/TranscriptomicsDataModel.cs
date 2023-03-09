namespace Unite.Genome.Feed.Web.Models.Transcriptomics;

public class TranscriptomicsDataModel
{
    private AnalysisModel _analysis;
    private AnalysedSampleModel _sample;
    private ExpressionModel[] _expressions;


    /// <summary>
    /// Analysis data
    /// </summary>
    public AnalysisModel Analysis { get => _analysis; set => _analysis = value; }

    /// <summary>
    /// Sample data
    /// </summary>
    public AnalysedSampleModel Sample { get => _sample; set => _sample = value; }

    /// <summary>
    /// Expression data
    /// </summary>
    public ExpressionModel[] Expressions { get => _expressions?.DistinctBy(e => e.GetContract()).ToArray(); set => _expressions = value; }
}
