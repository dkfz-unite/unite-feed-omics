using System.Text.Json.Serialization;
using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Variants.CNV.Enums;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV;

public class VariantAceSeqModel
{
    private Chromosome? _chromosome;
    private int? _start;
    private int? _end;
    private string _svType;
    private string _cnaType;
    private double? _c1Mean;
    private double? _c2Mean;
    private double? _tcnMean;
    private string _c1;
    private string _c2;
    private string _tcn;
    private double? _dhMax;

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
    /// Structural variant type (SV.Type)
    /// </summary>
    [JsonPropertyName("SV.type")]
    public string SvType { get => _svType?.Trim(); set => _svType = value; }

    /// <summary>
    /// Copy number alteration type (CNA.Type)
    /// </summary>
    [JsonPropertyName("CNA.type")]
    public string CnaType { get => _cnaType?.Trim(); set => _cnaType = value; }

    /// <summary>
    /// Mean number of copies in minor allele
    /// </summary>
    [JsonPropertyName("c1Mean")]
    public double? C1Mean { get => _c1Mean; set => _c1Mean = value; }

    /// <summary>
    /// Mean number of copies in major allele
    /// </summary>
    [JsonPropertyName("c2Mean")]
    public double? C2Mean { get => _c2Mean; set => _c2Mean = value; }

    /// <summary>
    /// Mean total number of copies (C1Mean + C2Mean)
    /// </summary>
    [JsonPropertyName("tcnMean")]
    public double? TcnMean { get => _tcnMean; set => _tcnMean = value; }

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
    public double? DhMax { get => _dhMax; set => _dhMax = value; }


    public SvType? GetSvType()
    {
        return Enum.TryParse<SvType>(SvType, out var value) ? value : null;
    }

    public CnaType? GetCnaType()
    {
        var valueString = CnaType?
            .Split(";", StringSplitOptions.RemoveEmptyEntries)?
            .Select(value => value.Trim())?
            .FirstOrDefault();

        return Enum.TryParse<CnaType>(valueString, out var value) ? value : null;
    }

    public bool? GetLoh()
    {
        return CnaType?.Contains("LOH") == true ? true : false;
    }

    public bool? GetHomoDel()
    {
        return CnaType?.Contains("HomoDel") == true ? true : false;
    }

    public int? GetC1()
    {
        return string.Equals(C1, "sub", StringComparison.InvariantCultureIgnoreCase) ? -1
             : int.TryParse(C1, out var value) ? value
             : null;
    }

    public int? GetC2()
    {
        return string.Equals(C2, "sub", StringComparison.InvariantCultureIgnoreCase) ? -1
             : int.TryParse(C2, out var value) ? value
             : null;
    }

    public int? GetTcn()
    {
        return string.Equals(Tcn, "sub", StringComparison.InvariantCultureIgnoreCase) ? -1
             : int.TryParse(Tcn, out var value) ? value
             : null;
    }
}
