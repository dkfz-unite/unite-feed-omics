using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Variants.SV.Enums;

namespace Unite.Genome.Feed.Web.Models.Variants.SV;

public class VariantModel
{
    /// <summary>
    /// Chromosome
    /// </summary>
    public Chromosome? Chromosome { get; set; }

    /// <summary>
    /// Chromosome region start
    /// </summary>
    public int? Start { get; set; }

    /// <summary>
    /// Chromosome region end
    /// </summary>
    public int? End { get; set; }

    /// <summary>
    /// New chromosome (for translocated or duplicated regions)
    /// </summary>
    public Chromosome? NewChromosome { get; set; }

    /// <summary>
    /// New chromosome region start (for translocated or duplicated regions)
    /// </summary>
    public double? NewStart { get; set; }

    /// <summary>
    /// New chromosome region end (for translocated or duplicated regions)
    /// </summary>
    public double? NewEnd { get; set; }

    /// <summary>
    /// Structural variant type
    /// </summary>
    public SvType? Type { get; set; }

    /// <summary>
    /// Reference genomic sequence of chromosome region
    /// </summary>
    public string Ref { get; set; }

    /// <summary>
    /// Alternate genomic sequence of chromosome region
    /// </summary>
    public string Alt { get; set; }
}
