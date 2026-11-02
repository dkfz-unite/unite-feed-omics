using Unite.Data.Entities.Omics.Analysis.Dna.Sm.Enums;

namespace Unite.Omics.Feed.Data.Models.Dna.Sm;

public record AnalysisModel : Models.AnalysisModel<VariantModel>
{
    public SmType[] EntriesScope { get; set; }
}
