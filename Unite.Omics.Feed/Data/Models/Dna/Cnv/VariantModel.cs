using Unite.Data.Entities.Omics.Analysis.Dna.Cnv.Enums;

namespace Unite.Omics.Feed.Data.Models.Dna.Cnv;

public class VariantModel : Dna.VariantModel
{
    public CnvType Type;
    public bool? Loh;
    public bool? Del;
    public double? C1Mean;
    public double? C2Mean;
    public double? TcnMean;
    public int? C1;
    public int? C2;
    public int? Tcn;
    public double? TcnRatio;
    public double? DhMax;
}
