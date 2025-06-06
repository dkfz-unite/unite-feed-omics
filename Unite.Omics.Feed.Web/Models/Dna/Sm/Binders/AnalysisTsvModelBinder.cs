using Unite.Essentials.Tsv;
using Unite.Omics.Feed.Web.Models.Base.Binders;

namespace Unite.Omics.Feed.Web.Models.Dna.Sm.Binders;

public class AnalysisTsvModelBinder : AnalysisTsvModelBinder<VariantModel>
{
    protected override ClassMap<VariantModel> CreateMap()
    {
        return new ClassMap<VariantModel>()
            .Map(entity => entity.Chromosome, "chromosome")
            .Map(entity => entity.Position, "position")
            .Map(entity => entity.Ref, "ref")
            .Map(entity => entity.Alt, "alt");
    }
}
