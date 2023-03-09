using Unite.Genome.Feed.Web.Models.Base;

namespace Unite.Genome.Feed.Web.Models.Variants;

/// <summary>
/// Variant type specific analysed sample model
/// </summary>
/// <typeparam name="TModel">Variant model type</typeparam>
public class AnalysedSampleModel<TModel> : SampleModel where TModel : class, IDistinctable, new()
{
    private string _matchedSampleId;
    private TModel[] _variants;


    /// <summary>
    /// Matched sample id
    /// </summary>
    public string MatchedSampleId { get => _matchedSampleId?.Trim(); set => _matchedSampleId = value; }

    /// <summary>
    /// List of variants appeared in the sample after analysis
    /// </summary>
    public TModel[] Variants { get => _variants?.DistinctBy(v => v.GetContract()).ToArray(); set => _variants = value; }
}
