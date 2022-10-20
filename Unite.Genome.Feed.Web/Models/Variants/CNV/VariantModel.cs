using Unite.Data.Entities.Genome.Enums;
using Unite.Data.Entities.Genome.Variants.CNV.Enums;

namespace Unite.Genome.Feed.Web.Models.Variants.CNV;

public class VariantModel
{
    private Chromosome? _chromosome;
    private int? _start;
    private int? _end;
    private SvType? _svType;
    private CnaType? _cnaType;
    private bool? _loh;
    private bool? _homoDel;
    private double? _c1Mean;
    private double? _c2Mean;
    private double? _tcnMean;
    private int? _c1;
    private int? _c2;
    private int? _tcn;
    private double? _dhMax;

    /// <summary>
    /// Chromosome
    /// </summary>
    public Chromosome? Chromosome { get => _chromosome; set => _chromosome = value; }

    /// <summary>
    /// Chromosome region start
    /// </summary>
    public int? Start { get => _start; set => _start = value; }

    /// <summary>
    /// Chromosome region end
    /// </summary>
    public int? End { get => _end; set => _end = value; }

    /// <summary>
    /// Structural variant type (SV.Type)
    /// </summary>
    public SvType? SvType { get => _svType; set => _svType = value; }

    /// <summary>
    /// Copy number alteration type (CNA.Type)
    /// </summary>
    public CnaType? CnaType { get => _cnaType; set => _cnaType = value; }

    /// <summary>
    /// Loss of heterozygosity (either C1 or C2 are 0)
    /// </summary>
    public bool? Loh { get => _loh; set => _loh = value; }

    /// <summary>
    /// Homozygous deletion (both C1 and C2 are 0)
    /// </summary>
    public bool? HomoDel { get => _homoDel; set => _homoDel = value; }

    /// <summary>
    /// Mean number of copies in minor allele
    /// </summary>
    public double? C1Mean { get => _c1Mean; set => _c1Mean = value; }

    /// <summary>
    /// Mean number of copies in major allele
    /// </summary>
    public double? C2Mean { get => _c2Mean; set => _c2Mean = value; }

    /// <summary>
    /// Mean total number of copies (C1Mean + C2Mean)
    /// </summary>
    public double? TcnMean { get => _tcnMean; set => _tcnMean = value; }

    /// <summary>
    /// Rounded number of copies in minor allele (-1 for subclonal values if values is 0.3+ far from closest integer)
    /// </summary>
    public int? C1 { get => _c1; set => _c1 = value; }

    /// <summary>
    /// Rounded number of copies in major allele (-1 for subclonal values if values is 0.3+ far from closest integer)
    /// </summary>
    public int? C2 { get => _c2; set => _c2 = value; }

    /// <summary>
    /// Rounded total number of copies (-1 for subclonal values if values is 0.3+ far from closest integer)
    /// </summary>
    public int? Tcn { get => _tcn; set => _tcn = value; }

    /// <summary>
    /// Estimated maximum decrease of heterozygosity
    /// </summary>
    public double? DhMax { get => _dhMax; set => _dhMax = value; }


    public VariantModel()
    {

    }

    public VariantModel(VariantAceSeqModel aceSeqModel)
    {
        Chromosome = aceSeqModel.Chromosome;
        Start = aceSeqModel.Start;
        End = aceSeqModel.End;
        SvType = aceSeqModel.GetSvType();
        CnaType = aceSeqModel.GetCnaType();
        Loh = aceSeqModel.GetLoh();
        HomoDel = aceSeqModel.GetHomoDel();
        C1Mean = aceSeqModel.C1Mean;
        C2Mean = aceSeqModel.C2Mean;
        TcnMean = aceSeqModel.TcnMean;
        C1 = aceSeqModel.GetC1();
        C2 = aceSeqModel.GetC2();
        Tcn = aceSeqModel.GetTcn();
        DhMax = aceSeqModel.DhMax;
    }
}
