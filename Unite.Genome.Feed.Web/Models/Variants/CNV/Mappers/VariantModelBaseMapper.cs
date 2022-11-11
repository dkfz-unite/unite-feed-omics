using Unite.Data.Entities.Genome.Variants.CNV.Enums;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Mappers;

public abstract class VariantModelBaseMapper
{
    /// <summary>
    /// Retrieves CNA.Type based on TCN or TCN mean and sample ploidy.
    /// </summary>
    /// <param name="tcn">TCN.</param>
    /// <param name="tcnMean">TCN mean.</param>
    /// <param name="ploidy">Sample ploidy.</param>
    /// <returns>CNA.Type based on TCN or TCN mean and sample ploidy.</returns>
    protected static CnaType? GetCnaType(int? tcn, double? tcnMean, double? ploidy)
    {
        if (tcn != null && tcn != -1 && ploidy != null)
        {
            return tcn == ploidy ? CnaType.Neutral : tcn < ploidy ? CnaType.Loss : CnaType.Gain;
        }
        else if (tcnMean != null && ploidy != null)
        {
            var delta = tcnMean.Value - ploidy.Value;

            if (Math.Abs(delta) < 0.3)
            {
                return CnaType.Neutral;
            }
            else if (Math.Abs(delta) > 0.7)
            {
                return delta > 0 ? CnaType.Gain : CnaType.Loss;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

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
