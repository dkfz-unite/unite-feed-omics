using Unite.Genome.Feed.Web.Models.Base;

namespace Unite.Genome.Feed.Web.Models.Variants;

/// <summary>
/// Variant type specific sequencing data model
/// </summary>
/// <typeparam name="TModel">Variant model type</typeparam>
public class SequencingDataModel<TModel> where TModel : class, IDistinctable, new()
{
    /// <summary>
    /// Analysis data
    /// </summary>
    public AnalysisModel Analysis { get; set; }

    /// <summary>
    /// Analysed samples data
    /// </summary>
    public AnalysedSampleModel<TModel>[] Samples { get; set; }


    public AnalysedSampleModel<TModel> FindSample(string id)
    {
        return Samples.FirstOrDefault(analysedSample => analysedSample.Id == id);
    }
}
