using Unite.Essentials.Tsv;
using Unite.Genome.Feed.Web.Models.Base.Binders;

namespace Unite.Genome.Feed.Web.Models.Variants.SSM.Binders;

public class TsvModelBinder : SequencingDataTsvModelBinder<VariantModel>
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
