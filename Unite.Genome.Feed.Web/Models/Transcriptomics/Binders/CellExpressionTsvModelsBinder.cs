using Unite.Essentials.Tsv;
using Unite.Genome.Feed.Web.Models.Base.Binders;

namespace Unite.Genome.Feed.Web.Models.Transcriptomics.Binders;

public class CellExpressionTsvModelsBinder : SequencingDataTsvModelBinder<CellExpressionModel>
{
    protected override ClassMap<CellExpressionModel> CreateMap()
    {
        return new ClassMap<CellExpressionModel>();
    }
}