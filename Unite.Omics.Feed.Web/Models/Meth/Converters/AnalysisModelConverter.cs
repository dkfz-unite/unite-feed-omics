using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Web.Models.Meth.Converters;

public class AnalysisModelConverter : Base.Converters.AnalysisModelConverter<Base.EmptyModel>
{
    protected override void MapEntries(Base.AnalysisModel<Base.EmptyModel> source, SampleModel target)
    {
    }
}
