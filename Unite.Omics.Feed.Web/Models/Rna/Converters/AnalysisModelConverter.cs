using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Web.Models.Rna.Converters;

public class AnalysisModelConverter : Base.Converters.AnalysisModelConverter<ExpressionModel>
{
    protected override void MapEntries(Base.AnalysisModel<ExpressionModel> source, SampleModel target)
    {
        
    }
}
