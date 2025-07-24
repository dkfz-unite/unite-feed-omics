using Unite.Data.Entities.Omics.Enums;
using Unite.Essentials.Extensions;
using Unite.Omics.Feed.Web.Models.Base.Readers;

namespace Unite.Omics.Feed.Web.Models.Dna.Sm.Readers.Vcf;

public class Reader : IReader<VariantModel>
{
    public string Format => "vcf";

    public VariantModel[] Read(StreamReader reader)
    {
        var variants = new List<VariantModel>();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (line.StartsWith('#'))
                continue;

            if (TryParseLine(line, out var variant))
                variants.Add(variant.Convert());
        }

        return variants.Where(variant => variant != null).ToArray();
    }

    private static bool TryParseLine(string line, out Variant variant)
    {
        var fields = line.Split('\t');

        if (TryParseChromosome(fields[0], out var chromosome))
        {
            variant = new Variant
            {
                Chromosome = chromosome.Value,
                Position = fields[1],
                Ref = fields[3],
                Alt = fields[4]
            };

            return true;
        }
        else
        {
            variant = null;

            return false;
        }
    }

    private static bool TryParseChromosome(string s, out Chromosome? result)
    {
        var values = Enum.GetValues(typeof(Chromosome)).Cast<Chromosome>().ToArray();

        foreach (var value in values)
        {
            if (string.Equals(value.ToDefinitionString(), s.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                result = value;
                return true;
            }
        }
        
        result = null;
        return false;
    }
}
