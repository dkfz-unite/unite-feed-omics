using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Models.Base.Binders;

namespace Unite.Omics.Feed.Web.Models.Dna.Cnv.Binders;

public class AnalysisTsvModelBinder : AnalysisTsvModelBinder<VariantModel>
{
    protected override ClassMap<VariantModel> CreateMap()
    {
        return new ClassMap<VariantModel>()
            .Map(entity => entity.Chromosome, "chromosome")
            .Map(entity => entity.Start, "start")
            .Map(entity => entity.End, "end")
            .Map(entity => entity.Type, "type")
            .Map(entity => entity.Loh, "loh")
            .Map(entity => entity.Del, "del")
            .Map(entity => entity.C1Mean, "c1_mean")
            .Map(entity => entity.C2Mean, "c2_mean")
            .Map(entity => entity.TcnMean, "tcn_mean")
            .Map(entity => entity.C1, "c1")
            .Map(entity => entity.C2, "c2")
            .Map(entity => entity.Tcn, "tcn");
    }
}
