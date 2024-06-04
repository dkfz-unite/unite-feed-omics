using Unite.Essentials.Tsv;
using Unite.Genome.Feed.Web.Models.Base.Binders;

namespace Unite.Genome.Feed.Web.Models.Rna.Binders;

public class ExpressionsTsvModelsBinder : AnalysisTsvModelBinder<ExpressionModel>
{
    protected override ClassMap<ExpressionModel> CreateMap()
    {
        return new ClassMap<ExpressionModel>()
            .Map(entity => entity.GeneId, "gene_id")
            .Map(entity => entity.GeneSymbol, "gene_symbol")
            .Map(entity => entity.TranscriptId, "transcript_id")
            .Map(entity => entity.TranscriptSymbol, "transcript_symbol")
            .Map(entity => entity.ExonicLength, "exonic_length")
            .Map(entity => entity.Reads, "reads");
    }
}
