namespace Unite.Omics.Feed.Web.Models.Dna.Cnv.Mappers;

public class VariantModelMapper : VariantModelBaseMapper
{
    public void Map(in VariantModel source, Data.Models.Dna.Cnv.VariantModel target, double? ploidy)
    {
        target.Chromosome = source.Chromosome.Value;
        target.Start = source.Start.Value;
        target.End = source.End.Value;
        target.Length = source.End.Value - source.Start.Value;

        target.C1Mean = source.C1Mean;
        target.C2Mean = source.C2Mean;
        target.TcnMean = source.TcnMean;
        target.C1 = source.C1;
        target.C2 = source.C2;
        target.Tcn = source.Tcn;
        target.TcnRatio = GetTcnRatio(target.Tcn, target.TcnMean, ploidy);

        // target.DhMax = source.DhMax;

        target.Type = source.Type.Value;
        target.Loh = source.Loh;
        target.Del = source.Del;
    }
}
