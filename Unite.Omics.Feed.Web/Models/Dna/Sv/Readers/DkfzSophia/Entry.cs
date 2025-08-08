using Unite.Data.Entities.Omics.Analysis.Dna.Sv.Enums;
using Unite.Data.Entities.Omics.Enums;
using Unite.Essentials.Extensions;
using Unite.Essentials.Tsv.Attributes;

namespace Unite.Omics.Feed.Web.Models.Dna.Sv.Readers.DkfzSophia;

public record Entry
{
    // TSV header starts from '#' which has to be ignored while reading the file.

    [Column("chrom1")]
    public string Chromosome1 { get; set; }

    [Column("start1")]
    public int? Start1 { get; set; }

    [Column("end1")]
    public int? End1 { get; set; }

    [Column("chrom2")]
    public string Chromosome2 { get; set; }

    [Column("start2")]
    public int? Start2 { get; set; }

    [Column("end2")]
    public int? End2 { get; set; }

    [Column("svtype")]
    public string Type { get; set; }

    [Column("eventInversion")]
    public string Inverted { get; set; }


    public VariantModel Convert()
    {
        try
        {
            return new VariantModel
            {
                Chromosome = GetChromosome(Chromosome1),
                Start = Start1,
                End = End1,
                OtherChromosome = GetChromosome(Chromosome2),
                OtherStart = Start2,
                OtherEnd = End2,
                Type = GetVariantType(this),
                Inverted = GetVariantInversion(this)
            };
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

    private static SvType GetVariantType(Entry variant)
    {
        if (string.Equals(variant.Type?.Trim(), "TRA", StringComparison.InvariantCultureIgnoreCase))
            return string.Equals(variant.Chromosome1, variant.Chromosome2, StringComparison.InvariantCultureIgnoreCase) ? SvType.ITX : SvType.CTX;

        var enumValues = Enum.GetValues(typeof(SvType)).Cast<SvType>().ToArray();

        foreach (var enumValue in enumValues)
        {
            if (string.Equals(enumValue.ToDefinitionString(), variant.Type.Trim(), StringComparison.InvariantCultureIgnoreCase))
                return enumValue;
        }

        throw new NotSupportedException($"SV type '{variant.Type}' is not supported");
    }

    private static bool? GetVariantInversion(Entry variant)
    {
        if (string.Equals(variant.Type?.Trim(), "TRA", StringComparison.InvariantCultureIgnoreCase))
        {
            if (string.Equals(variant.Inverted?.Trim(), "INV", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (string.Equals(variant.Inverted?.Trim(), "NORMAL", StringComparison.InvariantCultureIgnoreCase))
                return false;
        }

        return null;
    }
}
