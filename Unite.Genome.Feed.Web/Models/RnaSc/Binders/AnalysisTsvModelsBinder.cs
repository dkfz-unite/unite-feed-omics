using Unite.Essentials.Tsv;
using Unite.Genome.Feed.Web.Models.Base;
using Unite.Genome.Feed.Web.Models.Base.Binders;

namespace Unite.Genome.Feed.Web.Models.RnaSc.Binders;

public class AnalysisTsvModelsBinder : AnalysisTsvModelBinder<ResourceModel>
{
    protected override ClassMap<ResourceModel> CreateMap()
    {
        return new ClassMap<ResourceModel>()
            .Map(entity => entity.Type, "type")
            .Map(entity => entity.Format, "format")
            .Map(entity => entity.Url, "url");
    }
}
