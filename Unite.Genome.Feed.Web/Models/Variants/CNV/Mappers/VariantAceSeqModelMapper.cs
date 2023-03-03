namespace Unite.Genome.Feed.Web.Models.Variants.CNV.Mappers;

public class VariantAceSeqModelMapper : VariantModelBaseMapper
{
    public void Map(in VariantAceSeqModel source, Data.Models.Variants.CNV.VariantModel target, double? ploidy)
    {
        target.Chromosome = source.Chromosome.Value;
        target.Start = source.Start.Value;
        target.End = source.End.Value;
        target.Length = target.End - target.Start;

        target.C1Mean = source.GetC1Mean();
        target.C2Mean = source.GetC2Mean();
        target.TcnMean = source.GetTcnMean();

        target.C1 = source.GetC1();
        target.C2 = source.GetC2();
        target.Tcn = source.GetTcn();
        target.TcnRatio = GetTcnRatio(target.Tcn, target.TcnMean, ploidy);

        target.DhMax = source.GetDhMax();

        target.Type = source.GetCnvType() ?? GetCnvType(target.Tcn, target.TcnMean, ploidy);
        target.Loh = source.GetLoh();
        target.HomoDel = source.GetHomoDel();
    }
}
