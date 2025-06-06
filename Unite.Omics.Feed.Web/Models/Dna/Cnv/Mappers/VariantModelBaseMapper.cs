namespace Unite.Omics.Feed.Web.Models.Dna.Cnv.Mappers;

public abstract class VariantModelBaseMapper
{
    /// <summary>
    /// Retrieves TCN to ploidy ratio.
    /// </summary>
    /// <param name="tcn">TCN rouded value.</param>
    /// <param name="tcnMean">TCN mean value.</param>
    /// <param name="ploidy">Sample ploidy.</param>
    /// <returns>TCN to ploidy ratio if required data is available or null otherwise.</returns>
    protected static double? GetTcnRatio(int? tcn, double? tcnMean, double? ploidy)
    {
        if (ploidy != null && tcn != null && tcn != -1)
        {
            return tcn / ploidy;
        }
        else if (ploidy != null && tcnMean != null)
        {
            return tcnMean / ploidy;
        }
        else return null;
    }
}
