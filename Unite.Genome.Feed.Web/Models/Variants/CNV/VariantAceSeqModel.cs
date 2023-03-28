using System.Globalization;
using System.Text.Json.Serialization;
using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Variants.CNV.Enums;
using Unite.Genome.Feed.Web.Models.Base;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV;

public class VariantAceSeqModel : IDistinctable
{
    private Chromosome? _chromosome;
    private int? _start;
    private int? _end;
    private string _type;
    private string _c1Mean;
    private string _c2Mean;
    private string _tcnMean;
    private string _c1;
    private string _c2;
    private string _tcn;
    private string _dhMax;

    /// <summary>
    /// Chromosome
    /// </summary>
    [JsonPropertyName("chromosome")]
    public Chromosome? Chromosome { get => _chromosome; set => _chromosome = value; }

    /// <summary>
    /// Chromosome region start
    /// </summary>
    [JsonPropertyName("start")]
    public int? Start { get => _start; set => _start = value; }

    /// <summary>
    /// Chromosome region end
    /// </summary>
    [JsonPropertyName("end")]
    public int? End { get => _end; set => _end = value; }

    /// <summary>
    /// Copy number alteration type (CNA.Type)
    /// </summary>
    [JsonPropertyName("CNA.type")]
    public string Type { get => _type?.Trim(); set => _type = value; }

    /// <summary>
    /// Mean number of copies in minor allele
    /// </summary>
    [JsonPropertyName("c1Mean")]
    public string C1Mean { get => _c1Mean; set => _c1Mean = value; }

    /// <summary>
    /// Mean number of copies in major allele
    /// </summary>
    [JsonPropertyName("c2Mean")]
    public string C2Mean { get => _c2Mean; set => _c2Mean = value; }

    /// <summary>
    /// Mean total number of copies (C1Mean + C2Mean)
    /// </summary>
    [JsonPropertyName("tcnMean")]
    public string TcnMean { get => _tcnMean; set => _tcnMean = value; }

    /// <summary>
    /// Rounded number of copies in minor allele (-1 for subclonal values if values is 0.3+ far from closest integer)
    /// </summary>
    [JsonPropertyName("A")]
    public string C1 { get => _c1?.Trim(); set => _c1 = value; }

    /// <summary>
    /// Rounded number of copies in major allele (-1 for subclonal values if values is 0.3+ far from closest integer)
    /// </summary>
    [JsonPropertyName("B")]
    public string C2 { get => _c2?.Trim(); set => _c2 = value; }

    /// <summary>
    /// Rounded total number of copies (-1 for subclonal values if values is 0.3+ far from closest integer)
    /// </summary>
    [JsonPropertyName("TCN")]
    public string Tcn { get => _tcn?.Trim(); set => _tcn = value; }

    /// <summary>
    /// Estimated maximum decrease of heterozygosity
    /// </summary>
    [JsonPropertyName("dhMax")]
    public string DhMax { get => _dhMax; set => _dhMax = value; }



    public CnvType? GetCnvType()
    {
        var cnvTypeString = GetString(Type);

        var valueString = cnvTypeString?
            .Split(";", StringSplitOptions.RemoveEmptyEntries)?
            .Select(value => value.Trim())?
            .FirstOrDefault();

        return Enum.TryParse<CnvType>(valueString, out var value) ? value : null;
    }

    public bool? GetLoh()
    {
        var cnvTypeString = GetString(Type);

        return cnvTypeString?.Contains("LOH") == true ? true : false;
    }

    public bool? GetHomoDel()
    {
        var cnvTypeString = GetString(Type);

        return cnvTypeString?.Contains("HomoDel") == true ? true : false;
    }

    public double? GetC1Mean()
    {
        return GetDouble(C1Mean);
    }

    public double? GetC2Mean()
    {
        return GetDouble(C2Mean);
    }

    public double? GetTcnMean()
    {
        return GetDouble(TcnMean);
    }

    public double? GetDhMax()
    {
        return GetDouble(DhMax);
    }

    public int? GetC1()
    {
        return GetInteger(C1);
    }

    public int? GetC2()
    {
        return GetInteger(C2);
    }

    public int? GetTcn()
    {
        return GetInteger(Tcn);
    }

    public dynamic GetContract()
    {
        return new
        {
            Chromosome, Start, End, Type,
            C1Mean, C2Mean, TcnMean,
            C1, C2, Tcn,
            DhMax
        };
    }


    private static string GetString(string value)
    {
        return string.Equals(value, "NA", StringComparison.InvariantCultureIgnoreCase) ? null : value;
    }

    private static double? GetDouble(string value)
    {
        return string.Equals(value, "NA", StringComparison.InvariantCultureIgnoreCase) ? null
            : double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number) ? number >= 0 ? number : 0 : null;
    }

    private static int? GetInteger(string value)
    {
        return string.Equals(value, "NA", StringComparison.InvariantCultureIgnoreCase) ? null
             : string.Equals(value, "sub", StringComparison.InvariantCultureIgnoreCase) ? -1
             : int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number) ? number : null;
    }
}
