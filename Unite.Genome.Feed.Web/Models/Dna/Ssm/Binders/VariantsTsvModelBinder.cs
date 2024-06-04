using Unite.Essentials.Tsv;
using Unite.Genome.Feed.Web.Models.Base.Binders;

namespace Unite.Genome.Feed.Web.Models.Dna.Ssm.Binders;

public class VariantsTsvModelBinder : AnalysisTsvModelBinder<VariantModel>
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
