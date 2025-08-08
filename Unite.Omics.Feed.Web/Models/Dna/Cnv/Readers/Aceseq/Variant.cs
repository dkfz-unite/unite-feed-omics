using System.Globalization;
using Unite.Data.Entities.Omics.Analysis.Dna.Cnv.Enums;
using Unite.Data.Entities.Omics.Enums;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv.Attributes;

namespace Unite.Omics.Feed.Web.Models.Dna.Cnv.Readers.Aceseq;

public record Variant
{
    // TSV header starts from '#' which has to be ignored while reading the file.

    [Column("chromosome")]
    public string Chromosome { get; set; }

    [Column("start")]
    public string Start { get; set; }

    [Column("end")]
    public string End { get; set; }

    [Column("CNA.type")]
    public string CnaType { get; set; }

    [Column("c1Mean")]
    public string C1Mean { get; set; }

    [Column("c2Mean")]
    public string C2Mean { get; set; }

    [Column("tcnMean")]
    public string TcnMean { get; set; }

    [Column("B")]
    public string C1 { get; set; }

    [Column("A")]
    public string C2 { get; set; }

    [Column("TCN")]
    public string Tcn { get; set; }
    
    [Column("dhMax")]
    public string DhMax { get; set; }
    

    public VariantModel Convert()
    {
        try
        {
            var variant = new VariantModel
            {
                Chromosome = GetChromosome(Chromosome),
                Start = GetInteger(Start).Value,
                End = GetInteger(End).Value,
                Type = GetVariantType(CnaType),
                Loh = GetVariantLoh(CnaType),
                Del = GetVariantDel(CnaType),
                C1Mean = GetDouble(C1Mean),
                C2Mean = GetDouble(C2Mean),
                TcnMean = GetDouble(TcnMean),
                C1 = GetInteger(C1),
                C2 = GetInteger(C2),
                Tcn = GetInteger(Tcn),
                DhMax = GetDouble(DhMax)
            };

            if (variant.Start == variant.End)
                throw new InvalidDataException($"Start and End positions should not be equal. Start: {variant.Start}, End: {variant.End}");

            return variant;
        }
        catch
        {
            return null;
        }
    }

    private static Chromosome GetChromosome(string value)
    {
        var enumValues = Enum.GetValues(typeof(Chromosome)).Cast<Chromosome>().ToArray();

        foreach (var enumValue in enumValues)
        {
            if (string.Equals(enumValue.ToDefinitionString(), value.Trim(), StringComparison.InvariantCultureIgnoreCase))
                return enumValue;
        }

        throw new NotSupportedException($"Chromosome value '{value}' is not supported");
    }

    private static CnvType GetVariantType(string cnaType)
    {
        var comparison = StringComparison.InvariantCulture;

        var type = cnaType?
            .Split(";", StringSplitOptions.RemoveEmptyEntries)
            .Select(value => value.Trim())
            .FirstOrDefault();

        if (string.Equals(type, "AMP", comparison))
            return CnvType.Gain;
        else if (string.Equals(type, "DUP", comparison))
            return CnvType.Gain;
        else if (string.Equals(type, "TCNneutral", comparison))
            return CnvType.Neutral;
        else if (string.Equals(type, "DEL", comparison))
            return CnvType.Loss;
        else if (string.Equals(type, "HomoDel", comparison))
            return CnvType.Loss;
        else
            return CnvType.Undetermined;
    }

    private static bool? GetVariantLoh(string cnaType)
    {
        var comparison = StringComparison.InvariantCulture;

        return !string.IsNullOrWhiteSpace(cnaType) ? cnaType.Contains("LOH", comparison) == true : null;
    }

    private static bool? GetVariantDel(string cnaType)
    {
        var comparison = StringComparison.InvariantCulture;

        return !string.IsNullOrWhiteSpace(cnaType) ? cnaType.Contains("HomoDel", comparison) == true : null;
    }

    private static double? GetDouble(string value)
    {
        if (string.Equals(value, "NA", StringComparison.InvariantCultureIgnoreCase))
            return null;
        else if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
            return number >= 0 ? number : 0;
        else
            return null;
    }

    private static int? GetInteger(string value)
    {
        if (string.Equals(value, "NA", StringComparison.InvariantCultureIgnoreCase))
            return null;
        else if (string.Equals(value, "sub", StringComparison.InvariantCultureIgnoreCase))
            return -1;
        else if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
            return number >= 0 ? number : 0;
        else
            return null;
    }
}
