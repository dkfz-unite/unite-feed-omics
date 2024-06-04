using Unite.Essentials.Tsv;
using Unite.Genome.Feed.Web.Models.Base.Binders;

namespace Unite.Genome.Feed.Web.Models.RnaSc.Binders;

public class ExpressionsTsvModelsBinder : AnalysisTsvModelBinder<ExpressionModel>
{
    protected override ClassMap<ExpressionModel> CreateMap()
    {
        return new ClassMap<ExpressionModel>();
    }
}