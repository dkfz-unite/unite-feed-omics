using Unite.Data.Entities.Genome.Analysis.Dna.Cnv.Enums;

namespace Unite.Genome.Feed.Data.Models.Dna.Cnv;

public class VariantModel : Dna.VariantModel
{
    public CnvType Type { get; set; }
    public bool? Loh { get; set; }
    public bool? Del { get; set; }
    public double? C1Mean { get; set; }
    public double? C2Mean { get; set; }
    public double? TcnMean { get; set; }
    public int? C1 { get; set; }
    public int? C2 { get; set; }
    public int? Tcn { get; set; }
    public double? TcnRatio { get; set; }
    public double? DhMax { get; set; }
}
