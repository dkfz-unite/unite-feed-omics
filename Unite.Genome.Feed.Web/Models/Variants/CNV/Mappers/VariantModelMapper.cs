using Unite.Data.Entities.Genome.Variants.CNV.Enums;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Mappers;

public class VariantModelMapper
{
    public void Map(in VariantModel source, Data.Models.Variants.CNV.VariantModel target, double ploidy = 2)
    {
        target.Chromosome = source.Chromosome.Value;
        target.Start = source.Start.Value;
        target.End = source.End.Value;

        target.C1Mean = source.C1Mean;
        target.C2Mean = source.C2Mean;
        target.TcnMean = source.TcnMean;

        target.C1 = source.C1;
        target.C2 = source.C2;
        target.Tcn = source.Tcn;

        target.DhMax = source.DhMax;

        target.SvType = source.SvType;
        target.CnaType = source.CnaType ?? GetCnaType(target.Tcn, target.TcnMean, ploidy);
        target.Loh = target.Loh;
        target.HomoDel = target.HomoDel;
    }


    /// <summary>
    /// Retrieves CNA.Type based on TCN or TCN mean and sample ploidy.
    /// </summary>
    /// <param name="tcn">TCN.</param>
    /// <param name="tcnMean">TCN mean.</param>
    /// <param name="ploidy">Sample ploidy.</param>
    /// <returns>CNA.Type based on TCN or TCN mean and sample ploidy.</returns>
    private static CnaType? GetCnaType(double? tcn, double? tcnMean, double ploidy)
    {
        if (tcn != null && tcn != -1)
        {
            return tcn == ploidy ? CnaType.Neutral : tcn < ploidy ? CnaType.Loss : CnaType.Gain;
        }
        else if (tcnMean != null)
        {
            var delta = tcnMean.Value - ploidy;

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
}
