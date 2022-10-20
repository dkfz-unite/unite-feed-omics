using Unite.Data.Entities.Genome.Variants.CNV.Enums;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Mappers;

public class VariantModelMapper
{
    public void Map(in VariantModel source, Data.Models.Variants.CNV.VariantModel target, double ploidy)
    {
        target.Chromosome = source.Chromosome.Value;
        target.Start = source.Start.Value;
        target.End = source.End.Value;

        target.C1Mean = source.C1Mean.Value;
        target.C2Mean = source.C2Mean.Value;
        target.TcnMean = source.TcnMean ?? (source.C1Mean.Value + source.C2Mean.Value);

        target.C1 = source.C1 ?? RoundToInteger(target.C1Mean);
        target.C2 = source.C2 ?? RoundToInteger(target.C2Mean);
        target.Tcn = source.Tcn ?? RoundToInteger(target.TcnMean);

        target.DhMax = source.DhMax;

        target.SvType = source.SvType;
        target.CnaType = source.CnaType ?? GetCnaType(target.Tcn.Value, ploidy);
        target.Loh = target.C2 == 0 || target.C2 == 0;
        target.HomoDel = target.C1 == 0 && target.C2 == 0;
    }

    /// <summary>
    /// Retrieves CNA.Type based on TCN and sample ploidy
    /// </summary>
    /// <param name="tcn">TCN</param>
    /// <param name="ploidy">Sample ploidy</param>
    /// <returns>CNA.Type based on TCN and sample ploidy.</returns>
    private static CnaType? GetCnaType(int tcn, double ploidy)
    {
        //TODO: Ensure TCN.Type is calculated correctly if TCN is sub value (-1)
        return tcn == -1 ? null : tcn > ploidy ? CnaType.Gain : tcn < ploidy ? CnaType.Loss : CnaType.Neutral;
    }

    /// <summary>
    /// Rounds value based on maximum distance to nearest integer
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="maxDistance">Maximum distance to nearest integer</param>
    /// <returns>Rounded value if it is close enough to nearest integer, otherwise -1.</returns>
    private static int RoundToInteger(in double value, double maxDistance = 0.3)
    {
        return value - Math.Truncate(value) > maxDistance ? (int)Math.Round(value) : -1;
    }
}
