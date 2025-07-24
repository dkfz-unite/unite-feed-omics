using Unite.Data.Entities.Omics.Enums;
using Unite.Essentials.Extensions;

namespace Unite.Omics.Feed.Web.Models.Dna.Sm.Readers.Vcf;

public class Variant
{
    public Chromosome Chromosome { get; init; }
    public string Position { get; init; }
    public string Ref { get; init; }
    public string Alt { get; init; }


    /// <summary>
    /// Converts the VCF variant to UNITE SM (HGVS) format.
    /// </summary>
    public VariantModel Convert()
    {
        // SNV - no changes
        if (Ref.Length == 1 && Alt.Length == 1)
        {
            return new VariantModel
            {
                Chromosome = Chromosome,

                Position = Position,
                Ref = Ref,
                Alt = Alt
            };
        }
        // INS - conversion required
        else if (Ref.Length == 1 && Alt.Length > 1 && Alt.StartsWith(Ref))
        {
            return new VariantModel
            {
                Chromosome = Chromosome,

                // 1. Remove AlternateBase first nucleotide
                Alt = Alt[1..],
                // 2. Remove ReferenceBase
                Ref = null,
                // 3. Increase start position with 1
                Position = $"{int.Parse(Position) + 1}"
            };
        }
        // DEL - conversion required
        else if (Ref.Length > 1 && Alt.Length == 1 && Ref.StartsWith(Alt))
        {
            return new VariantModel
            {
                Chromosome = Chromosome,

                // 1. Remove ReferenceBase first nucleotide
                Ref = Ref[1..],
                // 2. Remove AlternateBase
                Alt = null,
                // 3. Increase start position with 1 and end position with (start + length of change)
                Position = Ref.Length == 2
                    ? $"{int.Parse(Position) + 1}"
                    : $"{int.Parse(Position) + 1}-{int.Parse(Position) + 1 + Ref[2..].Length}"
            };
        }
        // MNV - not supported
        else
        {
            return null;
        }
    }
}
