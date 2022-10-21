namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Mappers;

public class VariantAceSeqModelMapper
{
    public void Map(in VariantAceSeqModel source, Data.Models.Variants.CNV.VariantModel target, double ploidy)
    {
        target.Chromosome = source.Chromosome.Value;
        target.Start = source.Start.Value;
        target.End = source.End.Value;

        target.C1Mean = source.C1Mean.Value;
        target.C2Mean = source.C2Mean.Value;
        target.TcnMean = source.TcnMean.Value;

        target.C1 = source.GetC1();
        target.C2 = source.GetC2();
        target.Tcn = source.GetTcn();

        target.DhMax = source.DhMax;

        target.SvType = source.GetSvType();
        target.CnaType = source.GetCnaType();
        target.Loh = source.GetLoh();
        target.HomoDel = source.GetHomoDel();
    }
}
