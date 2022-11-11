using Unite.Data.Entities.Genome.Variants.CNV.Enums;

namespace Unite.Genome.Feed.Data.Models.Variants.CNV;

public class VariantModel : Variants.VariantModel
{
    public SvType? SvType { get; set; }
    public CnaType? CnaType { get; set; }
    public bool? Loh { get; set; }
    public bool? HomoDel { get; set; }
    public double? C1Mean { get; set; }
    public double? C2Mean { get; set; }
    public double? TcnMean { get; set; }
    public int? C1 { get; set; }
    public int? C2 { get; set; }
    public int? Tcn { get; set; }
    public double? TcnRatio { get; set; }
    public double? DhMax { get; set; }
}
