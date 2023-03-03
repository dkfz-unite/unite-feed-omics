using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Variants.CNV.Enums;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV;

public class VariantModel
{
    private Chromosome? _chromosome;
    private int? _start;
    private int? _end;
    private CnvType? _type;
    private bool? _loh;
    private bool? _homoDel;
    private double? _c1Mean;
    private double? _c2Mean;
    private double? _tcnMean;
    private int? _c1;
    private int? _c2;
    private int? _tcn;
    private double? _dhMax;

    /// <summary>
    /// Chromosome
    /// </summary>
    public Chromosome? Chromosome { get => _chromosome; set => _chromosome = value; }

    /// <summary>
    /// Chromosome region start
    /// </summary>
    public int? Start { get => _start; set => _start = value; }

    /// <summary>
    /// Chromosome region end
    /// </summary>
    public int? End { get => _end; set => _end = value; }

    /// <summary>
    /// Copy number alteration type (CNA.Type)
    /// </summary>
    public CnvType? Type { get => _type; set => _type = value; }

    /// <summary>
    /// Loss of heterozygosity (either C1 or C2 are 0)
    /// </summary>
    public bool? Loh { get => _loh ?? GetLoh(C1, C2); set => _loh = value; }

    /// <summary>
    /// Homozygous deletion (both C1 and C2 are 0)
    /// </summary>
    public bool? HomoDel { get => _homoDel ?? GetHomoDel(C1, C2); set => _homoDel = value; }

    /// <summary>
    /// Mean number of copies in minor allele
    /// </summary>
    public double? C1Mean { get => _c1Mean; set => _c1Mean = value; }

    /// <summary>
    /// Mean number of copies in major allele
    /// </summary>
    public double? C2Mean { get => _c2Mean; set => _c2Mean = value; }

    /// <summary>
    /// Mean total number of copies (C1Mean + C2Mean)
    /// </summary>
    public double? TcnMean { get => _tcnMean ?? C1Mean + C2Mean; set => _tcnMean = value; }

    /// <summary>
    /// Rounded number of copies in minor allele (-1 for subclonal values if values is 0.3+ far from closest integer)
    /// </summary>
    public int? C1 { get => _c1 ?? RoundToInteger(C1Mean); set => _c1 = value; }

    /// <summary>
    /// Rounded number of copies in major allele (-1 for subclonal values if values is 0.3+ far from closest integer)
    /// </summary>
    public int? C2 { get => _c2 ?? RoundToInteger(C2Mean); set => _c2 = value; }

    /// <summary>
    /// Rounded total number of copies (-1 for subclonal values if values is 0.3+ far from closest integer)
    /// </summary>
    public int? Tcn { get => _tcn ?? GetTcn(C1, C2) ?? RoundToInteger(TcnMean); set => _tcn = value; }

    /// <summary>
    /// Estimated maximum decrease of heterozygosity
    /// </summary>
    public double? DhMax { get => _dhMax; set => _dhMax = value; }



    private static bool? GetHomoDel(int? c1, int? c2)
    {
        return (c1 == -1 || c2 == -1) ? null : (c1 == 0 && c2 == 2);
    }

    private static bool? GetLoh(int? c1, int? c2)
    {
        return (c1 == -1 && c2 == -1) ? null : (c1 == 0 && c2 != 0) || (c2 == 0 && c1 != 0);
    }

    private static int? GetTcn(int? c1, int? c2)
    {
        return (c1 == -1 || c2 == -1) ? -1 : c1 + c2;
    }

    /// <summary>
    /// Rounds value based on maximum distance to nearest integer
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="maxDistance">Maximum distance to nearest integer</param>
    /// <returns>Rounded value if it is close enough to nearest integer, otherwise -1.</returns>
    private static int? RoundToInteger(in double? value, double maxDistance = 0.3)
    {
        if (value != null)
        {
            return value - Math.Truncate(value.Value) > maxDistance ? (int)Math.Round(value.Value) : -1;
        }
        else
        {
            return null;
        }

    }
}
